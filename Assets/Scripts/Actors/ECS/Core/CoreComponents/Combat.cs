using System.Collections;
using System;
using UnityEngine;

public class Combat : CoreComponent, IDamageable, IKnockbackable
{
    public event Action<float> OnDamage;

    [Header("Set variables")]
    [SerializeField] private bool isKnockable = true;
    [SerializeField] private bool isDamagable = true;

    [Header("Knockback variables")]
    [SerializeField] private Vector2 knockbackAngle = new Vector2(1,0);
    [SerializeField] private float knockbackStrength = 2;
    [SerializeField] private float maxKnockbackTime = 0.2f;

    private bool isKnockbackActive;
    private float knockbackStartTime;

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }

    private Movement movement;
    private Stats stats;

    public override void LogicUpdate()
    {
        CheckKnockback();
    }

    public void Damage(float amount)
    {
        if (isDamagable)
        {
            Stats?.DecreaseHealth(amount);

            OnDamage?.Invoke(amount);
        }
    }

    public void Knockback(int direction)
    {
        if (isKnockable)
        {
            Movement?.SetVelocity(knockbackStrength, knockbackAngle, direction);
            Movement.CanSetVelocity = false;
            isKnockbackActive = true;
            knockbackStartTime = Time.time;
        }
    }

    private void CheckKnockback()
    {
        if (isKnockbackActive && Time.time >= knockbackStartTime + maxKnockbackTime)
        {
            isKnockbackActive = false;
            Movement.CanSetVelocity = true;
        }
    }
}