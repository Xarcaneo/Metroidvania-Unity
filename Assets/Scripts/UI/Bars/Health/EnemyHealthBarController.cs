using UnityEngine;

/// <summary>
/// Specialized health bar controller for enemies that follows their position and fades out on death.
/// Provides health visualization with automatic positioning and death animation handling.
/// </summary>
public class EnemyHealthBarController : HealthBarController
{
    #region Variables
    [Header("Enemy References")]
    /// <summary>
    /// Reference to the enemy's body GameObject for position tracking.
    /// Required for the health bar to follow the enemy.
    /// </summary>
    [SerializeField] private GameObject body;

    /// <summary>
    /// Reference to the enemy's Stats component.
    /// Can be assigned directly or found through the body's Core component.
    /// </summary>
    [SerializeField] private Stats enemyStats;
    
    [Header("UI Settings")]
    /// <summary>
    /// Offset from the enemy's position where the health bar should be displayed.
    /// Adjust Y value to position the bar above the enemy.
    /// </summary>
    [SerializeField] private Vector3 yAdjust = new Vector3(-0.65f, 1, 0);

    /// <summary>
    /// Reference to the Animator component for fade out animation on death.
    /// Found automatically in Start().
    /// </summary>
    private Animator animator;
    #endregion

    #region Unity Methods
    /// <summary>
    /// Initializes components and sets up the enemy health bar.
    /// Gets Animator component and initializes enemy stats before base setup.
    /// </summary>
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        InitializeEnemyStats();
        base.Start();
    }

    /// <summary>
    /// Updates the health bar position to follow the enemy every frame after all Updates.
    /// Called in LateUpdate to ensure smooth following after enemy movement.
    /// </summary>
    protected virtual void LateUpdate()
    {
        UpdatePosition();
    }

    /// <summary>
    /// Updates the health bar UI and checks if the enemy has died.
    /// Triggers fade out animation if health reaches zero.
    /// </summary>
    public override void UpdateUI()
    {
        base.UpdateUI();
        CheckHealthState();
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Overrides base stats initialization since enemy stats are handled differently.
    /// Only sets up event subscriptions if stats are already initialized.
    /// </summary>
    protected override void InitializeStats()
    {
        // Override base initialization since enemy stats are handled differently
        if (stats != null)
        {
            SubscribeToEvents();
        }
    }

    /// <summary>
    /// Subscribes to enemy damage events.
    /// Enemies only subscribe to damage events as they don't have healing.
    /// </summary>
    protected override void SubscribeToEvents()
    {
        if (!stats) return;

        // Enemies only need damage event, they don't heal
        stats.Damaged += TakeDamage;
    }

    /// <summary>
    /// Unsubscribes from enemy damage events.
    /// Called on destruction and when enemy dies.
    /// </summary>
    protected override void UnsubscribeFromEvents()
    {
        if (!stats) return;

        stats.Damaged -= TakeDamage;
    }

    /// <summary>
    /// Validates all required components for the enemy health bar.
    /// Checks for body, core, stats, and animator components.
    /// Attempts to find missing components if possible.
    /// </summary>
    protected override void ValidateComponents()
    {
        base.ValidateComponents();

        if (!body)
        {
            Debug.LogError($"[EnemyHealthBarController] Body reference is missing on {gameObject.name}", this);
        }

        if (!enemyStats && body)
        {
            var core = body.GetComponent<Core>();
            if (!core)
            {
                Debug.LogError($"[EnemyHealthBarController] Core component not found on body {body.name}", this);
            }
            else
            {
                enemyStats = core.GetCoreComponent<Stats>();
                if (!enemyStats)
                {
                    Debug.LogError($"[EnemyHealthBarController] Stats component not found in Core on {body.name}", this);
                }
            }
        }

        animator = GetComponent<Animator>();
        if (!animator)
        {
            Debug.LogWarning($"[EnemyHealthBarController] Animator component is missing on {gameObject.name}", this);
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initializes enemy stats from either the inspector reference or by finding it in the body.
    /// Sets up initial health values and validates all required components.
    /// </summary>
    private void InitializeEnemyStats()
    {
        ValidateComponents();

        if (!enemyStats && body)
        {
            // Try to get stats if not assigned in inspector
            var core = body.GetComponent<Core>();
            if (core)
            {
                enemyStats = core.GetCoreComponent<Stats>();
            }
        }

        if (!enemyStats)
        {
            Debug.LogError($"[EnemyHealthBarController] Stats component not found for {gameObject.name}. Assign it in the inspector or ensure the body has a Core with Stats.", this);
            return;
        }

        stats = enemyStats;
        float maxHealthValue = stats.GetMaxHealth();
        UpdateMaxValue(maxHealthValue);
        currentValue = maxHealthValue;
    }

    /// <summary>
    /// Updates the health bar position to match the enemy body position.
    /// Applies yAdjust offset to position the bar above the enemy.
    /// </summary>
    private void UpdatePosition()
    {
        if (!body)
        {
            Debug.LogWarning($"[EnemyHealthBarController] Body reference is missing on {gameObject.name}", this);
            return;
        }

        transform.position = body.transform.position - yAdjust;
    }

    /// <summary>
    /// Checks if the enemy has died and triggers the fade out animation.
    /// Unsubscribes from events when health reaches zero.
    /// </summary>
    private void CheckHealthState()
    {
        if (currentValue <= 0 && animator != null)
        {
            UnsubscribeFromEvents();
            animator.Play("FadeOut");
        }
    }
    #endregion
}
