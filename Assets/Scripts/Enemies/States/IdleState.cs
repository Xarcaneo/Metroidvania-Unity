using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    protected D_IdleState stateData;

    protected bool flipAfterIdle;
    protected int playerDirection;

    protected bool isIdleTimeOver;
    protected bool isPlayerDetected;
    protected bool isDectectingLedge;
    protected bool isDetectingWall;
    protected bool isEnemyInRangeDetected;
    protected bool isPlayerInSight;

    protected float idleTime;

    public IdleState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData) : base(etity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerDetected = core.EntityDetector.GetEntityDetected();
        isDetectingWall = core.CollisionSenses.WallFront;
        isDectectingLedge = core.CollisionSenses.LedgeVertical;
        isEnemyInRangeDetected = core.AIMeleeAttackDetector.GetEntityDetected();
        playerDirection = core.EntityDetector.CheckFlipDirectionTowardsEntity();
        isPlayerInSight = core.AIRaycast.CheckRaycastCollision();
    }

    public override void Enter()
    {
        base.Enter();

        core.Movement.SetVelocityX(0f);
        isIdleTimeOver = false;
        SetRandomIdleTime();
    }

    public override void Exit()
    {
        base.Exit();

        if (flipAfterIdle)
        {
            core.Movement.Flip();
        }

        idleTime = 0.0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= startTime + idleTime)
        {
            isIdleTimeOver = true;
        }

        if (isPlayerDetected)
        {
            core.Movement.SetFlip(playerDirection);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void SetRandomIdleTime()
    {
        idleTime = Random.Range(stateData.minIdleTime, stateData.maxIdleTime);
    }
}