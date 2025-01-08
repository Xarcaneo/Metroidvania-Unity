using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the chase state behavior for the Skeleton enemy.
/// In this state, the skeleton actively pursues the player and can transition to attack or other states based on conditions.
/// </summary>
public class Skeleton_ChaseState : ChaseState
{
    private readonly Skeleton enemy;

    #region State Checks
    private bool isDetectingWall;
    private bool isDetectingLedge;
    private bool attackableTargetDetected;
    private IDamageable.DamageData m_damageData;
    #endregion

    #region Core Components
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    private Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;

    private EnemyDamageHitBox EnemyDamageHitBox { get => enemyDamageHitBox ?? core.GetCoreComponent(ref enemyDamageHitBox); }
    private EnemyDamageHitBox enemyDamageHitBox;
    #endregion

    /// <summary>
    /// Initializes a new instance of the Skeleton_ChaseState.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">The state machine managing this state</param>
    /// <param name="animBoolName">The animation boolean parameter name</param>
    /// <param name="stateData">Configuration data for the chase state</param>
    public Skeleton_ChaseState(Entity entity, StateMachine stateMachine, string animBoolName, D_ChaseState stateData) 
        : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = (Skeleton)entity;
    }

    /// <summary>
    /// Performs environment and target detection checks.
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isDetectingLedge = CollisionSenses.LedgeVertical;
            isDetectingWall = CollisionSenses.WallFront;
        }

        attackableTargetDetected = EnemyDamageHitBox.EntityInRange();
    }

    /// <summary>
    /// Called when entering the chase state.
    /// Initializes damage data for potential attacks.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        m_damageData.SetData(entity, Stats.GetAttack());
    }

    /// <summary>
    /// Updates the chase state logic each frame.
    /// Handles movement, attacks, and state transitions based on conditions.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        EnemyDamageHitBox?.MeleeAttack(m_damageData);
        EnemyDamageHitBox?.Knockback(m_damageData, Movement.FacingDirection);

        // Ensure skeleton is facing the player
        if (EntityDetector.entityToRight != Movement.FacingDirection)
            Movement.Flip();

        // Move towards the player
        Movement?.SetVelocityX(stateData.chaseSpeed * Movement.FacingDirection);

        // Handle state transitions
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
