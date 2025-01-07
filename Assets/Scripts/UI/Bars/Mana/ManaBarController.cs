using UnityEngine;

/// <summary>
/// Controls the mana bar UI, handling mana changes and visual updates with smooth transitions.
/// </summary>
public class ManaBarController : StatusBarController
{
    protected override void Awake()
    {
        restoreColor = Color.blue;
        depleteColor = Color.white;
        base.Awake();
    }

    protected override float GetMaxValueFromStats()
    {
        return stats.GetMaxMana();
    }

    protected override void SubscribeToEvents()
    {
        if (!stats) return;

        stats.ManaUsed += UseMana;
        stats.ManaRestored += RestoreMana;
    }

    protected override void UnsubscribeFromEvents()
    {
        if (!stats) return;

        stats.ManaUsed -= UseMana;
        stats.ManaRestored -= RestoreMana;
    }

    /// <summary>
    /// Reduces mana by the specified amount.
    /// </summary>
    public virtual void UseMana(float amount)
    {
        ReduceValue(amount);
    }

    /// <summary>
    /// Restores mana by the specified amount.
    /// </summary>
    public virtual void RestoreMana(float amount)
    {
        RestoreValue(amount);
    }
}
