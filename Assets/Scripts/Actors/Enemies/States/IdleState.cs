using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the idle state of an entity, where the entity remains idle for a random amount of time before transitioning to another state.
/// </summary>
public class IdleState : State
{
    /// <summary>
    /// The data associated with the idle state, containing parameters such as idle time range.
    /// </summary>
    protected D_IdleState stateData;

    /// <summary>
    /// Indicates whether the entity should flip its facing direction after the idle state.
    /// </summary>
    protected bool flipAfterIdle;

    /// <summary>
    /// Indicates if the idle time has finished.
    /// </summary>
    protected bool isIdleTimeOver;

    /// <summary>
    /// Indicates if the entity is detecting a ledge.
    /// </summary>
    protected bool isDectectingLedge;

    /// <summary>
    /// Indicates if the entity is detecting a wall.
    /// </summary>
    protected bool isDetectingWall;

    /// <summary>
    /// The amount of time the entity will remain idle.
    /// </summary>
    protected float idleTime;

    /// <summary>
    /// Gets the movement component of the entity, initializing it if necessary.
    /// </summary>
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }

    /// <summary>
    /// Gets the collision senses component of the entity, initializing it if necessary.
    /// </summary>
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }

    private Movement movement;
    private CollisionSenses collisionSenses;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdleState"/> class.
    /// </summary>
    /// <param name="etity">The entity associated with the state.</param>
    /// <param name="stateMachine">The state machine controlling the entity's states.</param>
    /// <param name="animBoolName">The name of the animation boolean associated with this state.</param>
    /// <param name="stateData">The data associated with the idle state, such as idle time range.</param>
    public IdleState(Entity etity, StateMachine stateMachine, string animBoolName, D_IdleState stateData) : base(etity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    /// <summary>
    /// Performs the necessary checks during the idle state, such as detecting walls or ledges.
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isDetectingWall = CollisionSenses.WallFront;
            isDectectingLedge = CollisionSenses.LedgeVertical;
        }
    }

    /// <summary>
    /// Called when entering the IdleState. Initializes necessary parameters for the idle state.
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        // Ensure no movement during idle state
        Movement?.SetVelocityX(0f);

        isIdleTimeOver = false;
        SetRandomIdleTime();
    }

    /// <summary>
    /// Called when exiting the IdleState. Resets state-specific values and performs any necessary cleanup.
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        if (flipAfterIdle)
        {
            // Flip the entity's facing direction after idle if needed
            Movement?.Flip();
        }

        // Reset idle time
        idleTime = 0.0f;
    }

    /// <summary>
    /// Called every frame to update the logic of the IdleState, such as checking if the idle time has passed.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);

        // Check if the idle time has passed
        if (Time.time >= startTime + idleTime)
        {
            isIdleTimeOver = true;
        }
    }

    /// <summary>
    /// Called every frame to update the physics during the idle state.
    /// </summary>
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    /// <summary>
    /// Sets a random idle time between the specified minimum and maximum idle time in the state data.
    /// </summary>
    private void SetRandomIdleTime()
    {
        idleTime = Random.Range(stateData.minIdleTime, stateData.maxIdleTime);
    }
}
