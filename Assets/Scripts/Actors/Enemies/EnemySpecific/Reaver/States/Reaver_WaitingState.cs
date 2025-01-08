using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the waiting behavior for the Reaver enemy.
/// Used when the enemy needs to pause and evaluate its next action.
/// </summary>
public class Reaver_WaitingState : WaitingState
{
    private readonly Reaver enemy;

    /// <summary>
    /// Initializes a new instance of the Reaver_WaitingState class.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">State machine managing this state</param>
    /// <param name="animBoolName">Animation boolean parameter name</param>
    /// <param name="enemy">Reference to the Reaver enemy instance</param>
    public Reaver_WaitingState(Entity entity, StateMachine stateMachine, string animBoolName, Reaver enemy) 
        : base(entity, stateMachine, animBoolName)
    {
        this.enemy = enemy;
    }

    /// <summary>
    /// Updates the logical state of the waiting behavior.
    /// Handles facing direction and state transitions based on current conditions.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (playerPosition != Movement.FacingDirection)
        {
            Movement.Flip();
        }

        if (!isPlayerDetected)
        {
            Movement.Flip();
            stateMachine.ChangeState(enemy.patrolState);
        }
        else if (attackableTargetDetected)
        {
            stateMachine.ChangeState(enemy.meleeAttackState);
        }
        else if(!isDetectingWall && isDetectingLedge)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
