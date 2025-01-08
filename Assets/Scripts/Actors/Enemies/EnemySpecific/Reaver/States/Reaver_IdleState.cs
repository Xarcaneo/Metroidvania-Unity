using UnityEngine;

/// <summary>
/// Handles the idle behavior for the Reaver enemy.
/// During idle, the enemy monitors for player presence and can transition to patrol or chase states.
/// </summary>
public class Reaver_IdleState : IdleState
{
    private readonly Reaver enemy;

    // Cached components
    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    // State variables
    private bool isPlayerDetected;

    /// <summary>
    /// Initializes a new instance of the Reaver_IdleState class.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">State machine managing this state</param>
    /// <param name="animBoolName">Animation boolean parameter name</param>
    /// <param name="stateData">Configuration data for the idle state</param>
    /// <param name="enemy">Reference to the Reaver enemy instance</param>
    public Reaver_IdleState(Entity entity, StateMachine stateMachine, string animBoolName, D_IdleState stateData, Reaver enemy) 
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
    /// Updates the logical state of the idle behavior.
    /// Handles transitions to patrol or chase states based on conditions.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            if (isPlayerDetected)
            {
                stateMachine.ChangeState(enemy.chaseState);
            }
            else if (isIdleTimeOver)
            {
                flipAfterIdle = true;
                stateMachine.ChangeState(enemy.patrolState);
            }
        }
    }
}
