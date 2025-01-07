using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the move state of an entity, where the entity moves based on its speed and direction.
/// </summary>
public class MoveState : State
{
    /// <summary>
    /// The data associated with the move state, containing parameters like movement speed.
    /// </summary>
    protected D_MoveState stateData;

    /// <summary>
    /// Indicates if the entity is detecting a wall in front.
    /// </summary>
    protected bool isDetectingWall;

    /// <summary>
    /// Indicates if the entity is detecting a ledge in front.
    /// </summary>
    protected bool isDetectingLedge;

    /// <summary>
    /// Gets the movement component of the entity, initializing it if necessary.
    /// </summary>
    protected Movement Movement
    {
        get => movement ?? core.GetCoreComponent(ref movement);
    }
    private Movement movement;

    /// <summary>
    /// Gets the collision senses component of the entity, initializing it if necessary.
    /// </summary>
    private CollisionSenses CollisionSenses
    {
        get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses);
    }
    private CollisionSenses collisionSenses;

    /// <summary>
    /// Initializes a new instance of the <see cref="MoveState"/> class.
    /// </summary>
    /// <param name="entity">The entity associated with the state.</param>
    /// <param name="stateMachine">The state machine controlling the entity's states.</param>
    /// <param name="animBoolName">The name of the animation boolean associated with this state.</param>
    /// <param name="stateData">The data containing movement parameters such as speed.</param>
    public MoveState(Entity entity, StateMachine stateMachine, string animBoolName, D_MoveState stateData)
        : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    /// <summary>
    /// Performs the necessary checks for environmental conditions, like detecting walls and ledges.
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses != null)
        {
            isDetectingLedge = CollisionSenses.LedgeVertical;
            isDetectingWall = CollisionSenses.WallFront;
        }
        else
        {
            Debug.LogWarning("CollisionSenses component is missing.");
        }
    }

    /// <summary>
    /// Called when entering the MoveState. Can be used to initialize state-specific settings or properties.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
    }

    /// <summary>
    /// Called when exiting the MoveState. Can be used for cleanup or resetting values.
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// Updates the state logic each frame. This method is used for state-specific logic, but currently has no additional logic.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    /// <summary>
    /// Updates the physics every frame, such as adjusting the velocity based on movement speed and direction.
    /// </summary>
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // Apply horizontal movement based on the facing direction and movement speed.
        Movement?.SetVelocityX(stateData.movementSpeed * Movement.FacingDirection);
    }
}
