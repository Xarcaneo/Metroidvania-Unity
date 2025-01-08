using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the waiting state behavior for the Skeleton enemy.
/// In this state, the skeleton waits while keeping track of the player and environment,
/// transitioning to other states based on conditions.
/// </summary>
public class Skeleton_WaitingState : WaitingState
{
    private readonly Skeleton enemy;

    /// <summary>
    /// Initializes a new instance of the Skeleton_WaitingState.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">The state machine managing this state</param>
    /// <param name="animBoolName">The animation boolean parameter name</param>
    public Skeleton_WaitingState(Entity entity, StateMachine stateMachine, string animBoolName) 
        : base(entity, stateMachine, animBoolName)
    {
        this.enemy = (Skeleton)entity;
    }

    /// <summary>
    /// Updates the waiting state logic each frame.
    /// Handles facing direction and state transitions based on player detection and environment.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Ensure skeleton is facing the player's position
        if (playerPosition != Movement.FacingDirection)
            Movement.Flip();

        // Return to patrol if player is no longer detected
        if (!isPlayerDetected)
        {
            Movement.Flip();
            stateMachine.ChangeState(enemy.patrolState);
        }
        // Resume chase if path is clear
        else if (!isDetectingWall && isDetectingLedge)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
