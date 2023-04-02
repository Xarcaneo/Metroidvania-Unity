using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : CoreComponent, IDamageable
{
    [SerializeField] public bool isDamagable = true;
    [SerializeField] private bool isBlockable = false;

    public event Action<float> OnDamage;
    public event Action OnSuccessfulBlock;
    public event Action OnAttackBlockedByDefender;

    private Block Block { get => block ?? core.GetCoreComponent(ref block); }
    private Block block;

    private Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;

    public void Damage(IDamageable.DamageData damageData)
    {
        if (isDamagable)
        {
            if (isBlockable && Block.isBlocking && Block.IsBetween(damageData.Source))
            {
                damageData.Source.Core.GetCoreComponent<DamageReceiver>().OnAttackBlockedByDefender?.Invoke();
                OnSuccessfulBlock?.Invoke();
                return;
            }
            else
            {
                Stats?.DecreaseHealth(damageData.DamageAmount);
                OnDamage?.Invoke(damageData.DamageAmount);
            }
        }
    }

    public void InstantKill()
    {
        Stats?.DecreaseHealth(stats.GetMaxHealth());
        OnDamage?.Invoke(stats.GetMaxHealth());
    }
}
