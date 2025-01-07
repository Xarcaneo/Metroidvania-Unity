using UnityEngine;

/// <summary>
/// Controls the health bar UI, handling health changes and visual updates with smooth transitions.
/// </summary>
public class HealthBarController : StatusBarController
{
    protected override void Awake()
    {
        restoreColor = Color.red;
        depleteColor = Color.white;
        base.Awake();
    }

    protected override float GetMaxValueFromStats()
    {
        return stats.GetMaxHealth();
    }

    protected override void SubscribeToEvents()
    {
        if (!stats) return;

        stats.Damaged += TakeDamage;
        stats.Healed += RestoreHealth;
    }

    protected override void UnsubscribeFromEvents()
    {
        if (!stats) return;

        stats.Damaged -= TakeDamage;
        stats.Healed -= RestoreHealth;
    }

    /// <summary>
    /// Reduces health by the specified amount of damage.
    /// </summary>
    public virtual void TakeDamage(float damage)
    {
        ReduceValue(damage);
    }

    /// <summary>
    /// Restores health by the specified amount.
    /// </summary>
    public virtual void RestoreHealth(float healAmount)
    {
        RestoreValue(healAmount);
    }
}
