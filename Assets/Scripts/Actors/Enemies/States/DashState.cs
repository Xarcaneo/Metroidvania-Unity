using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Represents the dash state of an entity, where the entity dashes in a direction.
/// </summary>
public class DashState : State
{
    /// <summary>
    /// The data associated with the dash state, containing dash-related parameters like velocity and detection range.
    /// </summary>
    protected readonly D_DashState stateData;

    /// <summary>
    /// The direction in which the entity is dashing. Default is 1 (forward).
    /// </summary>
    public int dashDirection = 1;

    // Variables to check player's current state
    protected bool isTouchingWall;
    protected bool isTouchingLedge;
    protected bool isPlayerDetected;

    /// <summary>
    /// Gets the movement component of the entity, initializing it if necessary.
    /// </summary>
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    /// <summary>
    /// Gets the collision senses component of the entity, initializing it if necessary.
    /// </summary>
    protected CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    /// <summary>
    /// Gets the entity detector component of the entity, initializing it if necessary.
    /// </summary>
    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashState"/> class.
    /// </summary>
    /// <param name="entity">The entity associated with the state.</param>
    /// <param name="stateMachine">The state machine controlling the entity's states.</param>
    /// <param name="animBoolName">The name of the animation boolean associated with this state.</param>
    /// <param name="stateData">The data associated with the dash state, containing dash parameters.</param>
    public DashState(Entity entity, StateMachine stateMachine, string animBoolName, D_DashState stateData)
        : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    /// <summary>
    /// Performs the necessary checks during the dash state, such as detecting walls, ledges, and the player.
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerDetected = EntityDetector.EntityInRange();

        if (CollisionSenses != null)
        {
            isTouchingWall = CollisionSenses.WallFront;
            isTouchingLedge = CollisionSenses.LedgeVertical;
        }
    }

    /// <summary>
    /// Called when entering the DashState. Initializes the start time for the dash.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        startTime = Time.time;
    }

    /// <summary>
    /// Checks if any entities are within range of the entity using a box overlap check.
    /// </summary>
    /// <returns>Returns true if an entity is detected within the specified range; otherwise, false.</returns>
    public bool EntityInRange()
    {
        Collider2D entityCollider = Physics2D.OverlapBox(entity.transform.position, new Vector2(stateData.detectionWidth,
            stateData.detectionHeight), 0, stateData.entityLayer);

        return entityCollider != null;
    }

    /// <summary>
    /// Checks if the dash will collide with obstacles, including walls, ledges, and the ground.
    /// </summary>
    /// <returns>Returns true if the dash will collide with an obstacle, indicating the dash is blocked.</returns>
    public bool DashBackCollision()
    {
        float raycastDistance = stateData.dashVelocity.x * stateData.dashTime;

        // Calculate the raycast direction based on the entity's facing direction
        Vector2 raycastDirection = new Vector2(-Movement.FacingDirection, 0);

        // Position for the bottom raycast
        Vector3 groundRaycastPosition = entity.transform.position + stateData.raycastBottomPosition;
        groundRaycastPosition.x += raycastDistance * -Movement.FacingDirection;

        // Perform raycasts to check for collisions
        bool bottomRaycastHit = Physics2D.Raycast(entity.transform.position + stateData.raycastBottomPosition, raycastDirection, raycastDistance, stateData.collisionLayer).collider != null;
        bool topRaycastHit = Physics2D.Raycast(entity.transform.position + stateData.raycastTopPosition, raycastDirection, raycastDistance, stateData.collisionLayer).collider != null;

        RaycastHit2D groundRaycastHit2D = Physics2D.Raycast(groundRaycastPosition, Vector2.down, stateData.groundRaycastDistance, stateData.collisionLayer);
        bool groundRaycastHit = groundRaycastHit2D.collider != null;

        Vector2 slopeNormalPerp = Vector2.Perpendicular(groundRaycastHit2D.normal).normalized;

        if (slopeNormalPerp.y > 0.1 || slopeNormalPerp.y < -0.1)
        {
            groundRaycastHit = false;
        }

        // Return true if any of the raycasts hit something, indicating a collision
        return bottomRaycastHit || topRaycastHit || !groundRaycastHit;
    }
}
