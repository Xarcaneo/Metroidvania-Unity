using UnityEngine;

/// <summary>
/// Specialized health bar controller for the player character.
/// Handles player-specific health bar behavior and game event integration.
/// </summary>
public class PlayerHealthBarController : HealthBarController
{
    private bool isNewSession = false;

    protected override void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Initializes the controller and subscribes to game events.
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

    /// <summary>
    /// Handles player spawn event by setting up stats and health.
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
    /// Handles player death event.
    /// </summary>
    private void OnPlayerDied()
    {
        isNewSession = true;
        UnsubscribeFromEvents();
    }

    /// <summary>
    /// Handles new session event.
    /// </summary>
    private void OnNewSession()
    {
        isNewSession = true;
    }

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
}
