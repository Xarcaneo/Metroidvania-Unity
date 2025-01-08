using UnityEngine;

/// <summary>
/// Handles the movement behavior for the Oculon enemy.
/// The Oculon moves between predefined target points and transitions to attack when detecting the player.
/// </summary>
public class Oculon_MoveState : MoveState
{
    private readonly Oculon enemy;
    private readonly Transform[] targetPoints;
    private int targetPointIndex;
    private bool isPlayerDetected;

    // Cached components
    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    private const float TARGET_POINT_THRESHOLD = 0.02f;

    /// <summary>
    /// Initializes a new instance of the Oculon_MoveState class.
    /// </summary>
    /// <param name="entity">The Oculon entity</param>
    /// <param name="stateMachine">State machine managing this state</param>
    /// <param name="animBoolName">Animation boolean parameter name</param>
    /// <param name="stateData">Movement configuration data</param>
    /// <param name="targetPoints">Array of points the Oculon will move between</param>
    public Oculon_MoveState(Entity entity, StateMachine stateMachine, string animBoolName, D_MoveState stateData, Transform[] targetPoints) 
        : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = (Oculon)entity;
        this.targetPoints = targetPoints;
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
    /// Updates the physics-based movement behavior.
    /// Moves the Oculon towards the current target point and handles state transitions.
    /// </summary>
    public override void PhysicsUpdate()
    {
        Vector2 currentPosition = enemy.transform.position;
        Vector2 targetPosition = targetPoints[targetPointIndex].position;

        // Move towards current target point
        MoveTowardsTarget(currentPosition, targetPosition);

        // Check if reached target point
        if (Vector2.Distance(currentPosition, targetPosition) < TARGET_POINT_THRESHOLD)
        {
            UpdateTargetPoint();
            TransitionState();
        }
    }

    /// <summary>
    /// Moves the Oculon towards the target position.
    /// </summary>
    private void MoveTowardsTarget(Vector2 currentPosition, Vector2 targetPosition)
    {
        int xDirection = targetPosition.x > currentPosition.x ? 1 : -1;
        Movement?.CheckIfShouldFlip(xDirection);
        
        enemy.transform.position = Vector2.MoveTowards(
            currentPosition, 
            targetPosition, 
            stateData.movementSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Updates the target point index, wrapping around to 0 if at the end.
    /// </summary>
    private void UpdateTargetPoint()
    {
        targetPointIndex = (targetPointIndex + 1) % targetPoints.Length;
    }

    /// <summary>
    /// Transitions to appropriate state based on player detection.
    /// </summary>
    private void TransitionState()
    {
        if (isPlayerDetected)
        {
            stateMachine.ChangeState(enemy.rangedAttackState);
        }
        else
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}
