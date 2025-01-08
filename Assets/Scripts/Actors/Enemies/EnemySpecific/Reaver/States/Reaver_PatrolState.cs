using UnityEngine;

/// <summary>
/// Represents the patrol state behavior for the Reaver enemy.
/// In this state, the Reaver moves back and forth within its patrol area.
/// </summary>
public class Reaver_PatrolState : MoveState
{
    private readonly Reaver enemy;
    private bool isPlayerDetected;

    #region Core Components
    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;
    #endregion

    /// <summary>
    /// Initializes a new instance of the Reaver_PatrolState.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">The state machine managing this state</param>
    /// <param name="animBoolName">The animation boolean parameter name</param>
    /// <param name="stateData">Configuration data for the patrol state</param>
    public Reaver_PatrolState(Entity entity, StateMachine stateMachine, string animBoolName, D_MoveState stateData) 
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
    /// Updates the patrol state logic each frame.
    /// Handles state transitions based on player detection and environment.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

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
