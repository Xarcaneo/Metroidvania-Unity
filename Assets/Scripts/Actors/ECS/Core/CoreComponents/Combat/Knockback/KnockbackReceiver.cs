using System;
using UnityEngine;

/// <summary>
/// Handles knockback effects on an entity, including receiving and applying knockback forces.
/// </summary>
public class KnockbackReceiver : Receiver, IKnockbackable
{
    #region Settings
    [Header("Knockback Settings")]
    [SerializeField, Tooltip("Whether this entity can be knocked back")]
    public bool isKnockable = true;

    [SerializeField, Tooltip("ScriptableObject containing knockback parameters")]
    private KnockbackData m_knockbackData;
    #endregion

    #region State
    public bool isKnockbackActive;
    private float knockbackStartTime;
    #endregion

    #region Core Component References
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;
    #endregion

    #region Events
    /// <summary>
    /// Triggered when knockback is successfully applied to this entity
    /// </summary>
    public event Action OnKnockback;
    #endregion

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        CheckKnockback();
    }

    /// <summary>
    /// Applies knockback force to the entity in the specified direction
    /// </summary>
    /// <param name="direction">Direction of the knockback (-1 or 1)</param>
    public void ApplyKnockback(int direction)
    {
        if (isKnockable && !IsImmune && Stats?.GetCurrentHealth() > 0)
        {
            Movement.SetVelocity(m_knockbackData.knockbackStrength, m_knockbackData.knockbackAngle, direction);
            Movement.CanSetVelocity = false;
            isKnockbackActive = true;
            knockbackStartTime = Time.time;
            OnKnockback?.Invoke();

            IsImmune = true;
            ImmunityEndTime = Time.time + immunityTime;
        }
    }

    /// <summary>
    /// Handles receiving knockback from a damage source
    /// </summary>
    /// <param name="damageData">Data about the damage causing knockback</param>
    /// <param name="direction">Direction of the knockback</param>
    public void ReceiveKnockback(IDamageable.DamageData damageData, int direction)
    {
        if (CheckBlock(damageData))
            return;

        ApplyKnockback(direction);
    }

    /// <summary>
    /// Applies knockback in the opposite direction of the entity's facing direction
    /// </summary>
    public void ReceiveKnockback() => ApplyKnockback(-Movement.FacingDirection);

    /// <summary>
    /// Checks and updates the knockback state
    /// </summary>
    private void CheckKnockback()
    {
        if (isKnockbackActive && Time.time >= knockbackStartTime + m_knockbackData.maxKnockbackTime)
        {
            isKnockbackActive = false;
            Movement.CanSetVelocity = true;
        }

        CheckImmunity();
    }
}