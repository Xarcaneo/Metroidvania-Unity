using System.Collections;
using System;
using UnityEngine;

public class KnockbackReceiver : CoreComponent, IKnockbackable
{
    [Header("Set variables")]
    [SerializeField] public bool isKnockable = true;
    [SerializeField] private bool isBlockable = false;

    [Header("Knockback Data")]
    [SerializeField] private KnockbackData m_knockbackData;

    public event Action OnKnockback;

    public bool isKnockbackActive;
    private float knockbackStartTime;

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    private Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;

    private Block Block { get => block ?? core.GetCoreComponent(ref block); }
    private Block block;

    public override void LogicUpdate()
    {
        CheckKnockback();
    }

    public void ApplyKnockback(int direction)
    {
        if (isBlockable && Block.isBlocking) return;

        if (isKnockable && Stats?.GetCurrentHealth() > 0)
        {
            Movement?.SetVelocity(m_knockbackData.knockbackStrength, m_knockbackData.knockbackAngle, direction);
            Movement.CanSetVelocity = false;
            isKnockbackActive = true;
            knockbackStartTime = Time.time;
            OnKnockback?.Invoke();
        }
    }

    public void ReceiveKnockback(int direction) => ApplyKnockback(direction);
    public void ReceiveKnockback() => ApplyKnockback(-Movement.FacingDirection);

    private void CheckKnockback()
    {
        if (isKnockbackActive && Time.time >= knockbackStartTime + m_knockbackData.maxKnockbackTime)
        {
            isKnockbackActive = false;
            Movement.CanSetVelocity = true;
        }
    }
}