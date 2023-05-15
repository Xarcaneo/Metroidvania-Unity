using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaver_ChaseState : ChaseState
{
    private Reaver enemy;

    private bool isDetectingWall;
    private bool isDetectingLedge;
    private bool attackableTargetDetected;

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    private EnemyDamageHitBox EnemyDamageHitBox { get => enemyDamageHitBox ?? core.GetCoreComponent(ref enemyDamageHitBox); }
    private EnemyDamageHitBox enemyDamageHitBox;

    public Reaver_ChaseState(Entity entity, StateMachine stateMachine, string animBoolName, D_ChaseState stateData) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = (Reaver)entity;
    }

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

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (EntityDetector.entityToRight != Movement.FacingDirection)
            Movement.Flip();

        Movement?.SetVelocityX(stateData.chaseSpeed * Movement.FacingDirection);

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
