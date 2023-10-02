using System;
using UnityEngine;

public abstract class Receiver : CoreComponent
{
    protected Block Block { get => block ?? core.GetCoreComponent(ref block); }
    private Block block;

    protected Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;

    protected bool IsImmune = false;
    protected float ImmunityEndTime;
    [SerializeField] protected float immunityTime = 0f;

    protected bool CheckBlock(IDamageable.DamageData damageData)
    {
        if (damageData.CanBlock && Block.isBlocking && Block.IsBetween(damageData.Source))
        {
            damageData.Source.Core.GetCoreComponent<Receiver>().OnAttackBlockedByDefender?.Invoke();
            OnSuccessfulBlock?.Invoke();
            return true;
        }

        return false;
    }

    public event Action OnSuccessfulBlock;
    public event Action OnAttackBlockedByDefender;

    protected void CheckImmunity()
    {
        if (IsImmune && Time.time >= ImmunityEndTime)
        {
            IsImmune = false;
        }
    }
}