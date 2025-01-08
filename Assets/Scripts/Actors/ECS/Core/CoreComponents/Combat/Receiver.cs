using System;
using UnityEngine;

/// <summary>
/// Base class for components that can receive combat interactions (damage, knockback, etc.).
/// Handles blocking and immunity mechanics.
/// </summary>
public abstract class Receiver : CoreComponent
{
    #region Core Component References
    protected Block Block { get => block ?? core.GetCoreComponent(ref block); }
    private Block block;

    protected Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;
    #endregion

    #region Immunity Settings
    [Header("Immunity Settings")]
    [SerializeField, Tooltip("Duration of immunity after receiving damage (in seconds)")]
    protected float immunityTime = 0f;

    protected bool IsImmune = false;
    protected float ImmunityEndTime;
    #endregion

    #region Events
    /// <summary>
    /// Triggered when this receiver successfully blocks an attack
    /// </summary>
    public event Action OnSuccessfulBlock;

    /// <summary>
    /// Triggered when this receiver's attack is blocked by a defender
    /// </summary>
    public event Action OnAttackBlockedByDefender;
    #endregion

    /// <summary>
    /// Checks if an incoming attack is blocked based on block component state and attacker position.
    /// Returns false if the entity doesn't have blocking capability.
    /// </summary>
    /// <param name="damageData">Data about the incoming attack</param>
    /// <returns>True if attack was blocked, false otherwise</returns>
    protected bool CheckBlock(IDamageable.DamageData damageData)
    {
        // If no Block component or damage can't be blocked, return false
        if (Block == null || !damageData.CanBlock) return false;

        // Check if blocking is active and angle is correct
        if (Block.isBlocking && Block.IsBetween(damageData.Source))
        {
            var attackerReceiver = damageData.Source.Core.GetCoreComponent<Receiver>();
            if (attackerReceiver != null)
            {
                attackerReceiver.OnAttackBlockedByDefender?.Invoke();
            }
            OnSuccessfulBlock?.Invoke();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Updates immunity status based on time
    /// </summary>
    protected void CheckImmunity()
    {
        if (IsImmune && Time.time >= ImmunityEndTime)
        {
            IsImmune = false;
        }
    }

    /// <summary>
    /// Starts immunity period for the configured duration
    /// </summary>
    protected void StartImmunity()
    {
        if (immunityTime <= 0) return;
        
        IsImmune = true;
        ImmunityEndTime = Time.time + immunityTime;
    }
}