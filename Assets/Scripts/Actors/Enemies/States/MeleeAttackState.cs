using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the melee attack state of an entity, where the entity performs a melee attack with its weapon.
/// </summary>
public class MeleeAttackState : AttackState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MeleeAttackState"/> class.
    /// </summary>
    /// <param name="entity">The entity associated with the state.</param>
    /// <param name="stateMachine">The state machine controlling the entity's states.</param>
    /// <param name="animBoolName">The name of the animation boolean associated with this state.</param>
    /// <param name="stateData">The data associated with the melee attack, containing attack parameters.</param>
    public MeleeAttackState(Entity entity, StateMachine stateMachine, string animBoolName, D_MeleeAttack stateData)
        : base(entity, stateMachine, animBoolName)
    {
    }

    /// <summary>
    /// Triggered when the animation action for the melee attack occurs. It performs the damage calculation and applies knockback.
    /// </summary>
    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        // Checks which IDamageable entities intersect with the weapon collider and damages them.
        EnemyDamageHitBox?.MeleeAttack(m_damageData);

        // Applies knockback based on the damage data and the entity's facing direction.
        EnemyDamageHitBox?.Knockback(m_damageData, Movement.FacingDirection);
    }
}
