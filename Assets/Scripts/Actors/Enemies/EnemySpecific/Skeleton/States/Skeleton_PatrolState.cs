using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the patrol state behavior for the Skeleton enemy.
/// In this state, the skeleton moves back and forth within its patrol area and can transition to chase or idle states.
/// </summary>
public class Skeleton_PatrolState : MoveState
{
    private readonly Skeleton enemy;
    private bool isPlayerDetected;

    #region Core Components
    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;
    #endregion

    /// <summary>
    /// Initializes a new instance of the Skeleton_PatrolState.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">The state machine managing this state</param>
    /// <param name="animBoolName">The animation boolean parameter name</param>
    /// <param name="stateData">Configuration data for the movement state</param>
    public Skeleton_PatrolState(Entity entity, StateMachine stateMachine, string animBoolName, D_MoveState stateData) 
        : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = entity as Skeleton;
    }

    /// <summary>
    /// Performs environment and player detection checks during patrol.
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();
        isPlayerDetected = EntityDetector.EntityInRange();
    }

    /// <summary>
    /// Updates the patrol state logic each frame.
    /// Handles state transitions based on player detection and environment checks.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Transition to chase state if player is detected
        if (isPlayerDetected)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
        // Transition to idle state if wall is detected or ledge is not detected
        else if (isDetectingWall || !isDetectingLedge)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
