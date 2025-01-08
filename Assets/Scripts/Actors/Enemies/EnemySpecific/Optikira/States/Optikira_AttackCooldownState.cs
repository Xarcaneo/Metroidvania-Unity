using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the cooldown period after an Optikira attack.
/// During this state, the enemy can transition to dash, ranged attack, or patrol states based on conditions.
/// </summary>
public class Optikira_AttackCooldownState : AttackCooldownState
{
    private readonly Optikira enemy;

    private bool isPlayerDetected;

    // Cached components
    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    /// <summary>
    /// Initializes a new instance of the Optikira_AttackCooldownState class.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">State machine managing this state</param>
    /// <param name="animBoolName">Animation boolean parameter name</param>
    /// <param name="stateData">Configuration data for the cooldown state</param>
    public Optikira_AttackCooldownState(Entity entity, StateMachine stateMachine, string animBoolName, D_AttackCooldownState stateData) 
        : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = (Optikira)entity;
    }

    /// <summary>
    /// Performs environmental checks and player detection.
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();
        isPlayerDetected = EntityDetector.EntityInRange();
    }

    /// <summary>
    /// Updates the logical state of the cooldown behavior.
    /// Handles facing the player and state transitions after cooldown.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (EntityDetector.entityToRight != Movement.FacingDirection)
        {
            Movement.Flip();
        }

        if (!isExitingState)
        {
            if (timeSinceEnteredState >= stateData.cooldownTime)
            {
                if (enemy.dashState.EntityInRange())
                {
                    stateMachine.ChangeState(enemy.dashState);
                }
                else if (isPlayerDetected)
                {
                    stateMachine.ChangeState(enemy.rangedAttackState);
                }
                else
                {
                    stateMachine.ChangeState(enemy.patrolState);
                }
            }
        }
    }
}
