using UnityEngine;

/// <summary>
/// Player-specific health bar controller that integrates with the game's event system.
/// Handles player lifecycle events and maintains health state between sessions.
/// </summary>
public class PlayerHealthBarController : HealthBarController
{
    #region Variables
    /// <summary>
    /// Tracks if we're starting a new game session.
    /// Used to determine whether to reset health to max.
    /// </summary>
    private bool isNewSession = false;
    #endregion

    #region Unity Methods
    /// <summary>
    /// Initializes the controller on start.
    /// Calls Initialize() to set up event subscriptions.
    /// </summary>
    protected override void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Cleans up event subscriptions when destroyed.
    /// Unsubscribes from all game events.
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();

        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onPlayerDied -= OnPlayerDied;
            GameEvents.Instance.onPlayerSpawned -= OnPlayerSpawned;
            GameEvents.Instance.onNewSession -= OnNewSession;
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Sets up the player health bar and subscribes to game events.
    /// Must be called before the health bar can function.
    /// </summary>
    public void Initialize()
    {
        if (GameEvents.Instance == null)
        {
            Debug.LogError($"[PlayerHealthBarController] GameEvents.Instance is null on {gameObject.name}", this);
            return;
        }

        GameEvents.Instance.onPlayerSpawned += OnPlayerSpawned;
        GameEvents.Instance.onPlayerDied += OnPlayerDied;
        GameEvents.Instance.onNewSession += OnNewSession;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Handles player spawn by setting up health stats.
    /// Resets health to max if new session, otherwise restores previous value.
    /// </summary>
    private void OnPlayerSpawned()
    {
        if (Player.Instance == null || Player.Instance.Core == null)
        {
            Debug.LogError($"[PlayerHealthBarController] Player instance or core is null on {gameObject.name}", this);
            return;
        }

        stats = Player.Instance.Core.GetCoreComponent<Stats>();
        if (stats == null)
        {
            Debug.LogError($"[PlayerHealthBarController] Could not get Stats component from player on {gameObject.name}", this);
            return;
        }

        if (isNewSession)
        {
            float maxHealthValue = stats.GetMaxHealth();
            UpdateMaxValue(maxHealthValue);
            currentValue = maxHealthValue;
            isNewSession = false;
        }
        else
        {
            stats.SetHealth(currentValue);
        }

        SubscribeToEvents();
    }

    /// <summary>
    /// Handles player death by marking for new session.
    /// Unsubscribes from events until player respawns.
    /// </summary>
    private void OnPlayerDied()
    {
        isNewSession = true;
        UnsubscribeFromEvents();
    }

    /// <summary>
    /// Handles new game session event.
    /// Marks that health should be reset to maximum on next spawn.
    /// </summary>
    private void OnNewSession()
    {
        isNewSession = true;
    }
    #endregion
}
