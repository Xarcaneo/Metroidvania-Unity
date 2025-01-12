using UnityEngine;

/// <summary>
/// Base mana bar controller that provides mana-specific UI visualization.
/// Implements mana events and color schemes for usage and restoration.
/// </summary>
public class ManaBarController : StatusBarController
{
    #region Unity Methods
    /// <summary>
    /// Sets up mana bar colors and initializes base components.
    /// Blue for restoration, white for depletion.
    /// </summary>
    protected override void Awake()
    {
        restoreColor = Color.blue;
        depleteColor = Color.white;
        base.Awake();
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Gets maximum mana value from the Stats component.
    /// Used to set the bar's maximum fill amount.
    /// </summary>
    /// <returns>Maximum possible mana value</returns>
    protected override float GetMaxValueFromStats()
    {
        return stats.GetMaxMana();
    }

    /// <summary>
    /// Gets current mana value from the Stats component.
    /// Used to set the bar's current fill amount.
    /// </summary>
    /// <returns>Current mana value</returns>
    protected override float GetCurrentValueFromStats()
    {
        return stats.GetCurrentMana();
    }

    /// <summary>
    /// Subscribes to mana-related events from Stats.
    /// Listens for both mana use and restoration events.
    /// </summary>
    protected override void SubscribeToEvents()
    {
        if (!stats) return;

        stats.ManaUsed += UseMana;
        stats.ManaRestored += RestoreMana;
    }

    /// <summary>
    /// Unsubscribes from mana-related events.
    /// Called on destruction or when explicitly needed.
    /// </summary>
    protected override void UnsubscribeFromEvents()
    {
        if (!stats) return;

        stats.ManaUsed -= UseMana;
        stats.ManaRestored -= RestoreMana;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Handles mana consumption by reducing mana value.
    /// Triggers smooth bar transition with depletion color.
    /// </summary>
    /// <param name="amount">Amount of mana to consume</param>
    public virtual void UseMana(float amount)
    {
        ReduceValue(amount);
    }

    /// <summary>
    /// Handles mana restoration by increasing mana value.
    /// Triggers smooth bar transition with restore color.
    /// </summary>
    /// <param name="amount">Amount of mana to restore</param>
    public virtual void RestoreMana(float amount)
    {
        RestoreValue(amount);
    }
    #endregion
}
