using UnityEngine;

/// <summary>
/// Handles the idle behavior for the Optikira enemy.
/// During idle, the enemy monitors for player presence and can transition to patrol or attack states.
/// </summary>
public class Optikira_IdleState : IdleState
{
    private readonly Optikira enemy;

    private bool isPlayerDetected;

    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    /// <summary>
    /// Initializes a new instance of the Optikira_IdleState class.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">State machine managing this state</param>
    /// <param name="animBoolName">Animation boolean parameter name</param>
    /// <param name="stateData">Configuration data for the idle state</param>
    public Optikira_IdleState(Entity entity, StateMachine stateMachine, string animBoolName, D_IdleState stateData) 
        : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = entity as Optikira;
    }

    /// <summary>
    /// Performs checks for the idle state.
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerDetected = EntityDetector.EntityInRange();
    }

    /// <summary>
    /// Updates the logical state of the idle behavior.
    /// Handles transitions to patrol or attack states based on conditions.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            // Check for player detection and idle time completion
            if (isPlayerDetected)
            {
                stateMachine.ChangeState(enemy.rangedAttackState);
            }
            else if (isIdleTimeOver)
            {
                flipAfterIdle = true;
                stateMachine.ChangeState(enemy.patrolState);
            }
        }
    }
}
