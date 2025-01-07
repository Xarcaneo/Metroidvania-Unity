using UnityEngine;

/// <summary>
/// Health bar controller specifically for enemies, with position adjustment and fade out animation.
/// </summary>
public class EnemyHealthBarController : HealthBarController
{
    [Header("Enemy References")]
    [SerializeField] private GameObject body;
    [SerializeField] private Stats enemyStats;
    
    [Header("UI Settings")]
    [SerializeField] private Vector3 yAdjust = new Vector3(-0.65f, 1, 0);

    private Animator animator;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        InitializeEnemyStats();
        base.Start();
    }

    protected virtual void LateUpdate()
    {
        UpdatePosition();
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        CheckHealthState();
    }

    protected override void InitializeStats()
    {
        // Override base initialization since enemy stats are handled differently
        if (stats != null)
        {
            SubscribeToEvents();
        }
    }

    /// <summary>
    /// Initializes stats from the enemy body.
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

    protected override void SubscribeToEvents()
    {
        if (!stats) return;

        // Enemies only need damage event, they don't heal
        stats.Damaged += TakeDamage;
    }

    protected override void UnsubscribeFromEvents()
    {
        if (!stats) return;

        stats.Damaged -= TakeDamage;
    }

    /// <summary>
    /// Updates the UI position to follow the enemy body.
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
    /// Checks health state and triggers fade out animation if dead.
    /// </summary>
    private void CheckHealthState()
    {
        if (currentValue <= 0 && animator != null)
        {
            UnsubscribeFromEvents();
            animator.Play("FadeOut");
        }
    }

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
}
