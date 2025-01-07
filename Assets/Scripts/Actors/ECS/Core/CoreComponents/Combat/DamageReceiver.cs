using System;
using UnityEngine;

/// <summary>
/// Handles receiving and processing damage for an entity.
/// Includes damage blocking, immunity periods, and health management.
/// </summary>
public class DamageReceiver : Receiver, IDamageable
{
    #region Damage Settings
    [Header("Damage Settings")]
    [SerializeField, Tooltip("Whether this entity can receive damage")]
    public bool isDamagable = true;  // Keeping old name for compatibility
    #endregion

    #region Events
    /// <summary>
    /// Triggered when damage is successfully applied. Parameter is the amount of damage dealt.
    /// </summary>
    public event Action<float> OnDamage;
    #endregion

    /// <summary>
    /// Processes incoming damage, applying it if conditions are met
    /// </summary>
    /// <param name="damageData">Data about the incoming damage</param>
    public void Damage(IDamageable.DamageData damageData)
    {
        if (!isDamagable || IsImmune || Stats == null) return;

        // Check if damage is blocked
        if (CheckBlock(damageData)) return;

        // Only apply damage if entity is alive
        if (Stats.GetCurrentHealth() <= 0) return;

        // Apply damage and start immunity period
        Stats.DecreaseHealth(damageData.DamageAmount);
        OnDamage?.Invoke(damageData.DamageAmount);
        StartImmunity();
    }

    /// <summary>
    /// Instantly reduces health to zero
    /// </summary>
    public void InstantKill()
    {
        if (Stats == null) return;

        float maxHealth = Stats.GetMaxHealth();
        Stats.DecreaseHealth(maxHealth);
        OnDamage?.Invoke(maxHealth);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        CheckImmunity();
    }
}
