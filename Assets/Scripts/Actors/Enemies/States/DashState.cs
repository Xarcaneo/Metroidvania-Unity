using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DashState : State
{
    protected readonly D_DashState stateData;

    public int dashDirection = 1;

    //Variables to check player's current state
    protected bool isTouchingWall;
    protected bool isTouchingLedge;
    protected bool isPlayerDetected;

    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;
    protected CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;
    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;


    public DashState(Entity entity, StateMachine stateMachine, string animBoolName, D_DashState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerDetected = EntityDetector.EntityInRange();

        if (CollisionSenses)
        {
            isTouchingWall = CollisionSenses.WallFront;
            isTouchingLedge = CollisionSenses.LedgeVertical;
        }
    }

    public override void Enter()
    {
        base.Enter();

        startTime = Time.time;
    }

    public bool EntityInRange()
    {
        // Check for any collider in the detection area that matches the entityLayer
        Collider2D entityCollider = Physics2D.OverlapBox(entity.transform.position, new Vector2(stateData.detectionWidth, 
            stateData.detectionHeight), 0, stateData.entityLayer);

        if (entityCollider != null) return true;
        else return false;
    }

    public bool DashBackCollision()
    {
        // Calculate the raycast distance based on dash velocity and time
        float raycastDistance = stateData.dashVelocity.x * stateData.dashTime;

        // Calculate the raycast direction based on the facing direction
        Vector2 raycastDirection = new Vector2(-Movement.FacingDirection, 0);

        // Calculate the position for the bottom raycast
        Vector3 groundRaycastPosition = entity.transform.position + stateData.raycastBottomPosition;
        groundRaycastPosition.x += raycastDistance * -Movement.FacingDirection;

        // Perform raycasts in different directions and check for collisions
        bool bottomRaycastHit = Physics2D.Raycast(entity.transform.position + stateData.raycastBottomPosition, raycastDirection, raycastDistance, stateData.collisionLayer).collider != null;
        bool topRaycastHit = Physics2D.Raycast(entity.transform.position + stateData.raycastTopPosition, raycastDirection, raycastDistance, stateData.collisionLayer).collider != null;
        
        // Perform the ground raycast and store the result in a RaycastHit2D variable
        RaycastHit2D groundRaycastHit2D = Physics2D.Raycast(groundRaycastPosition, Vector2.down, stateData.groundRaycastDistance, stateData.collisionLayer);

        // Assign the result of the ground raycast to the boolean variable
        bool groundRaycastHit = groundRaycastHit2D.collider != null;

        Vector2 slopeNormalPerp = Vector2.Perpendicular(groundRaycastHit2D.normal).normalized;

        if (slopeNormalPerp.y > 0.1 || slopeNormalPerp.y < -0.1) 
        {
            groundRaycastHit = false;
        }

        // Draw the raycasts for visualization
        //Debug.DrawRay(entity.transform.position + stateData.raycastBottomPosition, raycastDirection * raycastDistance, bottomRaycastHit ? Color.red : Color.yellow);
        //Debug.DrawRay(entity.transform.position + stateData.raycastTopPosition, raycastDirection * raycastDistance, topRaycastHit ? Color.red : Color.blue);
        //Debug.DrawRay(groundRaycastPosition, Vector2.down * stateData.groundRaycastDistance, groundRaycastHit ? Color.red : Color.green);

        // Return true if any of the raycasts hit something, indicating a collision
        return bottomRaycastHit || topRaycastHit || !groundRaycastHit;
    }
}
