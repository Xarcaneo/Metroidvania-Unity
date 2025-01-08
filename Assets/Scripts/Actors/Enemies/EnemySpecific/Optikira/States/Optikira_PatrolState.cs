using UnityEngine;

/// <summary>
/// Handles the patrol behavior for the Optikira enemy.
/// The enemy moves back and forth, checking for player presence and obstacles.
/// </summary>
public class Optikira_PatrolState : MoveState
{
    private readonly Optikira enemy;

    private bool isPlayerDetected;

    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    /// <summary>
    /// Initializes a new instance of the Optikira_PatrolState class.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">State machine managing this state</param>
    /// <param name="animBoolName">Animation boolean parameter name</param>
    /// <param name="stateData">Configuration data for the patrol state</param>
    public Optikira_PatrolState(Entity entity, StateMachine stateMachine, string animBoolName, D_MoveState stateData) 
        : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = (Optikira)entity;
    }

    /// <summary>
    /// Performs checks for the patrol behavior.
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
            // Handle movement and state transitions
            Movement?.SetVelocityX(stateData.movementSpeed * Movement.FacingDirection);

            if (isPlayerDetected)
            {
                stateMachine.ChangeState(enemy.rangedAttackState);
            }
            else if (isDetectingWall || !isDetectingLedge)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }
    }
}
