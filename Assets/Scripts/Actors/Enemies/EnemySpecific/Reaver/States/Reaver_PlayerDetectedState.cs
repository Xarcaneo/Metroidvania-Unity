using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaver_PlayerDetectedState : PlayerDetectedState
{
    private Reaver enemy;
    private D_Reaver_PlayerDetectedState stateData;

    private bool isDetectingWall;
    private bool isDetectingLedge;
    private bool attackableTargetDetected;

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    private EnemyDamageHitBox EnemyDamageHitBox { get => enemyDamageHitBox ?? core.GetCoreComponent(ref enemyDamageHitBox); }
    private EnemyDamageHitBox enemyDamageHitBox;

    public Reaver_PlayerDetectedState(Entity entity, StateMachine stateMachine, string animBoolName, D_Reaver_PlayerDetectedState stateData, Reaver enemy) : base(entity, stateMachine, animBoolName)
    {
        this.enemy = enemy;
        this.stateData = stateData;
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
            stateMachine.ChangeState(enemy.moveState);
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
