using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the idle state behavior for the Skeleton enemy.
/// In this state, the skeleton remains stationary for a period and can transition to patrol or chase states.
/// </summary>
public class Skeleton_IdleState : IdleState
{
    private readonly Skeleton enemy;
    private bool isPlayerDetected;

    #region Core Components
    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;
    #endregion

    /// <summary>
    /// Initializes a new instance of the Skeleton_IdleState.
    /// </summary>
    /// <param name="etity">The entity this state belongs to</param>
    /// <param name="stateMachine">The state machine managing this state</param>
    /// <param name="animBoolName">The animation boolean parameter name</param>
    /// <param name="stateData">Configuration data for the idle state</param>
    /// <param name="enemy">Reference to the skeleton enemy instance</param>
    public Skeleton_IdleState(Entity etity, StateMachine stateMachine, string animBoolName, D_IdleState stateData, Skeleton enemy) 
        : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    /// <summary>
    /// Performs environment and player detection checks.
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();
        isPlayerDetected = EntityDetector.EntityInRange();
    }

    /// <summary>
    /// Called when entering the idle state.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
    }

    /// <summary>
    /// Updates the idle state logic each frame.
    /// Handles state transitions based on idle time and player detection.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Transition to patrol state if idle time is over
        if (isIdleTimeOver)
        {
            flipAfterIdle = true;
            stateMachine.ChangeState(enemy.patrolState);
        }
        // Transition to chase state if player is detected
        else if (isPlayerDetected)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
