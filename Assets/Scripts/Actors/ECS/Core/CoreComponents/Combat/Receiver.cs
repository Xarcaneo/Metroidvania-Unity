using System;
using UnityEngine;

public abstract class Receiver : CoreComponent
{
    [SerializeField] protected bool isBlockable = false;

    protected Block Block { get => block ?? core.GetCoreComponent(ref block); }
    private Block block;

    protected Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;

    protected bool IsImmune = false;
    protected float ImmunityEndTime;
    [SerializeField] protected float immunityTime = 0f;

    protected void CheckBlock(IDamageable.DamageData damageData)
    {
        if (isBlockable && Block.isBlocking && Block.IsBetween(damageData.Source))
        {
            damageData.Source.Core.GetCoreComponent<Receiver>().OnAttackBlockedByDefender?.Invoke();
            OnSuccessfulBlock?.Invoke();
        }
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