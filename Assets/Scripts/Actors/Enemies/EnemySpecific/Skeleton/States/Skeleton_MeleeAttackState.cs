using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the melee attack state behavior for the Skeleton enemy.
/// In this state, the skeleton performs melee attacks when in range of the player.
/// </summary>
public class Skeleton_MeleeAttackState : AttackState
{
    private readonly Skeleton enemy;
    private bool attackableTargetDetected;

    /// <summary>
    /// Initializes a new instance of the Skeleton_MeleeAttackState.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">The state machine managing this state</param>
    /// <param name="animBoolName">The animation boolean parameter name</param>
    public Skeleton_MeleeAttackState(Entity entity, StateMachine stateMachine, string animBoolName) 
        : base(entity, stateMachine, animBoolName)
    {
        this.enemy = (Skeleton)entity;
    }

    /// <summary>
    /// Performs target detection checks during the attack state.
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();
        attackableTargetDetected = EnemyDamageHitBox.EntityInRange();
    }

    /// <summary>
    /// Updates the melee attack state logic each frame.
    /// Handles attack execution, facing direction, and state transitions.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Execute melee attack and knockback
        EnemyDamageHitBox?.MeleeAttack(m_damageData);
        EnemyDamageHitBox?.Knockback(m_damageData, Movement.FacingDirection);

        // Ensure skeleton is facing the target
        if (Movement.FacingDirection != EnemyDamageHitBox.entityToRight)
        {
            Movement.Flip();
        }

        // Transition to patrol state if target is no longer in range
        if (!attackableTargetDetected)
        {
            stateMachine.ChangeState(enemy.patrolState);
        }
    }
}
