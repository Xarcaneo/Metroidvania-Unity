using UnityEngine;

/// <summary>
/// Handles the patrol behavior for the Reaver enemy.
/// The enemy moves back and forth between points, checking for obstacles and player presence.
/// </summary>
public class Reaver_PatrolState : MoveState
{
    private readonly Reaver enemy;

    // Cached components
    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    private bool isPlayerDetected;

    /// <summary>
    /// Initializes a new instance of the Reaver_PatrolState class.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">State machine managing this state</param>
    /// <param name="animBoolName">Animation boolean parameter name</param>
    /// <param name="stateData">Configuration data for the patrol state</param>
    /// <param name="enemy">Reference to the Reaver enemy instance</param>
    public Reaver_PatrolState(Entity entity, StateMachine stateMachine, string animBoolName, D_MoveState stateData, Reaver enemy) 
        : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
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
    /// Updates the logical state of the patrol behavior.
    /// Handles movement and state transitions based on conditions.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            Movement?.SetVelocityX(stateData.movementSpeed * Movement.FacingDirection);

            if (isPlayerDetected)
            {
                stateMachine.ChangeState(enemy.chaseState);
            }
            else if (isDetectingWall || !isDetectingLedge)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }
    }
}
