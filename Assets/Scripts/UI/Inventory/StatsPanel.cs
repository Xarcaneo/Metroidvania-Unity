using Opsive.UltimateInventorySystem.UI.Panels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the UI panel that displays player statistics.
/// Updates automatically when stats change and handles player lifecycle events.
/// </summary>
public class StatsPanel : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>
    /// Text component displaying player's attack stat.
    /// </summary>
    [SerializeField] private TextMeshProUGUI AttackText;

    /// <summary>
    /// Text component displaying player's defense stat.
    /// </summary>
    [SerializeField] private TextMeshProUGUI DefenseText;

    /// <summary>
    /// Text component displaying player's maximum health.
    /// </summary>
    [SerializeField] private TextMeshProUGUI HealthText;
    #endregion

    #region Private Fields
    /// <summary>
    /// Reference to the player's stats component.
    /// </summary>
    private Stats stats;

    /// <summary>
    /// Tracks if the panel is currently initialized.
    /// </summary>
    private bool isInitialized;
    #endregion

    #region Unity Event Functions
    /// <summary>
    /// Cleans up event subscriptions when component is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        GameEvents.Instance.onPlayerSpawned -= OnPlayerSpawned;
        UnsubscribeFromEvents();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Initializes the stats panel and subscribes to player events.
    /// Should be called when the panel is first activated.
    /// </summary>
    public void Initialize()
    {
        if (isInitialized)
        {
            Debug.LogWarning("[StatsPanel] Panel already initialized!");
            return;
        }

        if (GameEvents.Instance == null)
        {
            Debug.LogError("[StatsPanel] GameEvents instance is null!");
            return;
        }

        GameEvents.Instance.onPlayerSpawned += OnPlayerSpawned;
        isInitialized = true;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Handles player spawn event by setting up stats tracking.
    /// </summary>
    private void OnPlayerSpawned()
    {
        if (Player.Instance == null || Player.Instance.Core == null)
        {
            Debug.LogError("[StatsPanel] Player instance or core is null!");
            return;
        }

        UnsubscribeFromEvents();
        
        stats = Player.Instance.Core.GetCoreComponent<Stats>();
        if (stats == null)
        {
            Debug.LogError("[StatsPanel] Could not find Stats component on player!");
            return;
        }

        stats.StatsUpdated += Draw;
        Draw();
    }

    /// <summary>
    /// Updates the UI with current stat values.
    /// </summary>
    private void Draw()
    {
        if (stats == null)
        {
            Debug.LogWarning("[StatsPanel] Cannot draw stats: Stats component is null!");
            return;
        }

        if (AttackText != null) AttackText.text = stats.GetAttack().ToString();
        if (DefenseText != null) DefenseText.text = stats.GetDefense().ToString();
        if (HealthText != null) HealthText.text = stats.GetMaxHealth().ToString();
    }

    /// <summary>
    /// Unsubscribes from all events to prevent memory leaks.
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        if (stats != null)
        {
            stats.StatsUpdated -= Draw;
        }
    }
    #endregion
}
