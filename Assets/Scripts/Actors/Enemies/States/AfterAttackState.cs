using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterAttackState : State
{
    protected bool isStateTimeOver;
    protected float stateDurationTime;

    protected bool isEnemyInRangeDetected;
    protected bool isPlayerDetected;
    protected bool isDectectingLedge;
    protected bool isPlayerInSight;

    protected int playerDirection;

    public AfterAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isEnemyInRangeDetected = core.AIMeleeAttackDetector.GetEntityDetected();
        isPlayerDetected = core.EntityDetector.GetEntityDetected();
        isDectectingLedge = core.CollisionSenses.LedgeVertical;
        playerDirection = core.EntityDetector.CheckFlipDirectionTowardsEntity();
        isPlayerInSight = core.AIRaycast.CheckRaycastCollision();
    }

    public override void Enter()
    {
        base.Enter();
        core.Movement.SetVelocityX(0f);
        isStateTimeOver = false;
    }

    public override void Exit()
    {
        base.Exit();

        stateDurationTime = 0.0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        core.Movement.SetVelocityX(0f);

        if (Time.time >= startTime + stateDurationTime)
        {
            isStateTimeOver = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public void SetStateDurationTime( float stateDurationTime)
    {
        this.stateDurationTime = stateDurationTime;
    }
}
