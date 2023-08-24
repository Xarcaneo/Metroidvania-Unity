using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : Receiver, IDamageable
{
    [SerializeField] public bool isDamagable = true;

    public event Action<float> OnDamage;

    public void Damage(IDamageable.DamageData damageData)
    {
        if (isDamagable && !IsImmune)
        {
            if (CheckBlock(damageData))
                return;

            if (Stats?.GetCurrentHealth() > 0)
            {
                Stats.DecreaseHealth(damageData.DamageAmount);
                OnDamage?.Invoke(damageData.DamageAmount);

                IsImmune = true;
                ImmunityEndTime = Time.time + immunityTime;
            }
        }
    }

    public void InstantKill()
    {
        Stats?.DecreaseHealth(Stats.GetMaxHealth());
        OnDamage?.Invoke(Stats.GetMaxHealth());
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        CheckImmunity();
    }
}
