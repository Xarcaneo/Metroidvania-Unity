using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a waiting state for the enemy, typically used when the enemy is idle or monitoring its surroundings.
/// </summary>
public class WaitingState : State
{
    /// <summary>
    /// Indicates if the player has been detected by the enemy.
    /// </summary>
    protected bool isPlayerDetected;

    /// <summary>
    /// Indicates if the enemy is detecting a wall in front.
    /// </summary>
    protected bool isDetectingWall;

    /// <summary>
    /// Indicates if the enemy is detecting a ledge in front.
    /// </summary>
    protected bool isDetectingLedge;

    /// <summary>
    /// Represents the current position of the player relative to the enemy.
    /// </summary>
    protected int playerPosition;

    /// <summary>
    /// Indicates if an attackable target is detected within the enemy's attack range.
    /// </summary>
    protected bool attackableTargetDetected;

    /// <summary>
    /// Gets the movement component of the entity, initializing it if necessary.
    /// </summary>
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    /// <summary>
    /// Gets the entity detector component of the entity, initializing it if necessary.
    /// </summary>
    protected EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    /// <summary>
    /// Gets the enemy damage hitbox component of the entity, initializing it if necessary.
    /// </summary>
    private EnemyDamageHitBox EnemyDamageHitBox { get => enemyDamageHitBox ?? core.GetCoreComponent(ref enemyDamageHitBox); }
    private EnemyDamageHitBox enemyDamageHitBox;

    /// <summary>
    /// Gets the collision senses component of the entity, initializing it if necessary.
    /// </summary>
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    /// <summary>
    /// Initializes a new instance of the <see cref="WaitingState"/> class.
    /// </summary>
    /// <param name="entity">The entity associated with the state.</param>
    /// <param name="stateMachine">The state machine controlling the entity's states.</param>
    /// <param name="animBoolName">The name of the animation boolean associated with this state.</param>
    public WaitingState(Entity entity, StateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }

    /// <summary>
    /// Performs the necessary checks for detecting the player and other environmental factors.
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerDetected = EntityDetector.EntityInRange();
        playerPosition = entityDetector.entityToRight;
        attackableTargetDetected = EnemyDamageHitBox.EntityInRange();

        if (CollisionSenses)
        {
            isDetectingLedge = CollisionSenses.LedgeVertical;
            isDetectingWall = CollisionSenses.WallFront;
        }
    }

    /// <summary>
    /// Called when the state is entered. Initializes the state, such as setting velocity.
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        // Ensure no movement during the waiting state.
        Movement?.SetVelocityX(0f);
    }

    /// <summary>
    /// Called when the state is exited. Can be used to reset or clean up the state.
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// Called during the logic update phase of the game loop. Can be used to handle state-specific logic.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Ensure no movement during the waiting state.
        Movement?.SetVelocityX(0f);
    }

    /// <summary>
    /// Called during the physics update phase of the game loop. Can be used to handle state-specific physics logic.
    /// </summary>
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
