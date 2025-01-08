using UnityEngine;

/// <summary>
/// Represents the idle state behavior for the Reaver enemy.
/// In this state, the Reaver remains stationary for a period and can transition to other states.
/// </summary>
public class Reaver_IdleState : IdleState
{
    private readonly Reaver enemy;
    private bool isPlayerDetected;

    #region Core Components
    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;
    #endregion

    /// <summary>
    /// Initializes a new instance of the Reaver_IdleState.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">The state machine managing this state</param>
    /// <param name="animBoolName">The animation boolean parameter name</param>
    /// <param name="stateData">Configuration data for the idle state</param>
    public Reaver_IdleState(Entity entity, StateMachine stateMachine, string animBoolName, D_IdleState stateData) 
        : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = entity as Reaver;
    }

    /// <summary>
    /// Performs environment and player detection checks.
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();
        isPlayerDetected = EntityDetector.EntityInRange();
    }

    /// <summary>
    /// Updates the idle state logic each frame.
    /// Handles state transitions based on idle time and player detection.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isIdleTimeOver)
        {
            flipAfterIdle = true;
            stateMachine.ChangeState(enemy.patrolState);
        }
        else if (isPlayerDetected)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
