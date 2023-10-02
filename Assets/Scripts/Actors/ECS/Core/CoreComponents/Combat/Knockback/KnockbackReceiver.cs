using System.Collections;
using System;
using UnityEngine;

public class KnockbackReceiver : Receiver, IKnockbackable
{
    [SerializeField] public bool isKnockable = true;
    [SerializeField] private KnockbackData m_knockbackData;

    public event Action OnKnockback;

    public bool isKnockbackActive;
    private float knockbackStartTime;

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        CheckKnockback();
    }

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

    public void ReceiveKnockback(IDamageable.DamageData damageData, int direction)
    {
        if (CheckBlock(damageData))
            return;

        ApplyKnockback(direction);
    }
    public void ReceiveKnockback() => ApplyKnockback(-Movement.FacingDirection);

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