using UnityEngine;

/// <summary>
/// Handles the chase behavior for the Reaver enemy.
/// The Reaver will actively pursue the player while checking for obstacles and attack opportunities.
/// </summary>
public class Reaver_ChaseState : ChaseState
{
    private readonly Reaver enemy;

    #region State Variables
    private bool isDetectingWall;
    private bool isDetectingLedge;
    private bool attackableTargetDetected;
    #endregion

    #region Component References
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    private EnemyDamageHitBox EnemyDamageHitBox { get => enemyDamageHitBox ?? core.GetCoreComponent(ref enemyDamageHitBox); }
    private EnemyDamageHitBox enemyDamageHitBox;
    #endregion

    /// <summary>
    /// Initializes a new instance of the Reaver_ChaseState class.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">State machine managing this state</param>
    /// <param name="animBoolName">Animation boolean parameter name</param>
    /// <param name="stateData">Configuration data for the chase state</param>
    public Reaver_ChaseState(Entity entity, StateMachine stateMachine, string animBoolName, D_ChaseState stateData) 
        : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = (Reaver)entity;
    }

    /// <summary>
    /// Performs environmental checks for obstacles and attackable targets.
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();
        CheckEnvironment();
        CheckAttackRange();
    }

    /// <summary>
    /// Updates the logical state of the chase behavior.
    /// Handles movement and state transitions based on conditions.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        UpdateMovement();
        CheckStateTransitions();
    }

    /// <summary>
    /// Checks for environmental obstacles like walls and ledges.
    /// </summary>
    private void CheckEnvironment()
    {
        if (CollisionSenses)
        {
            isDetectingLedge = CollisionSenses.LedgeVertical;
            isDetectingWall = CollisionSenses.WallFront;
        }
    }

    /// <summary>
    /// Checks if a target is within attack range.
    /// </summary>
    private void CheckAttackRange()
    {
        attackableTargetDetected = EnemyDamageHitBox.EntityInRange();
    }

    /// <summary>
    /// Updates the enemy's movement direction and velocity.
    /// </summary>
    private void UpdateMovement()
    {
        if (EntityDetector.entityToRight != Movement.FacingDirection)
        {
            Movement.Flip();
        }
        Movement?.SetVelocityX(stateData.chaseSpeed * Movement.FacingDirection);
    }

    /// <summary>
    /// Checks and handles state transitions based on current conditions.
    /// </summary>
    private void CheckStateTransitions()
    {
        if (!isPlayerDetected)
        {
            stateMachine.ChangeState(enemy.patrolState);
        }
        else if (isDetectingWall || !isDetectingLedge)
        {
            stateMachine.ChangeState(enemy.waitingState);
        }
        else if (attackableTargetDetected)
        {
            stateMachine.ChangeState(enemy.meleeAttackState);
        }
    }
}
