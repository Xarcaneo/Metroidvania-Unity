using UnityEngine;

/// <summary>
/// Base health bar controller that provides health-specific UI visualization.
/// Implements health events and color schemes for damage and healing.
/// </summary>
public class HealthBarController : StatusBarController
{
    #region Unity Methods
    /// <summary>
    /// Sets up health bar colors and initializes base components.
    /// Red for healing, white for damage.
    /// </summary>
    protected override void Awake()
    {
        restoreColor = Color.red;
        depleteColor = Color.white;
        base.Awake();
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Gets maximum health value from the Stats component.
    /// Used to set the bar's maximum fill amount.
    /// </summary>
    /// <returns>Maximum possible health value</returns>
    protected override float GetMaxValueFromStats()
    {
        return stats.GetMaxHealth();
    }

    /// <summary>
    /// Gets current health value from the Stats component.
    /// Used to set the bar's current fill amount.
    /// </summary>
    /// <returns>Current health value</returns>
    protected override float GetCurrentValueFromStats()
    {
        return stats.GetCurrentHealth();
    }

    /// <summary>
    /// Subscribes to health-related events from Stats.
    /// Listens for both damage and healing events.
    /// </summary>
    protected override void SubscribeToEvents()
    {
        if (!stats) return;

        stats.Damaged += TakeDamage;
        stats.Healed += RestoreHealth;
    }

    /// <summary>
    /// Unsubscribes from health-related events.
    /// Called on destruction or when explicitly needed.
    /// </summary>
    protected override void UnsubscribeFromEvents()
    {
        if (!stats) return;

        stats.Damaged -= TakeDamage;
        stats.Healed -= RestoreHealth;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Handles damage taken by reducing health value.
    /// Triggers smooth bar transition with damage color.
    /// </summary>
    /// <param name="damage">Amount of damage to apply</param>
    public virtual void TakeDamage(float damage)
    {
        ReduceValue(damage);
    }

    /// <summary>
    /// Handles healing by restoring health value.
    /// Triggers smooth bar transition with heal color.
    /// </summary>
    /// <param name="healAmount">Amount of health to restore</param>
    public virtual void RestoreHealth(float healAmount)
    {
        RestoreValue(healAmount);
    }
    #endregion
}
