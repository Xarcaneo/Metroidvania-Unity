using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    protected D_IdleState stateData;

    protected bool flipAfterIdle;
    protected bool isIdleTimeOver;
    protected bool isPlayerDetected;
    protected bool isDectectingLedge;
    protected bool isEnemyInRangeDetected;
    protected int playerDirection;

    protected float idleTime;

    public IdleState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData) : base(etity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerDetected = core.EntityDetector.GetEntityDetected();
        isDectectingLedge = core.CollisionSenses.LedgeVertical;
        isEnemyInRangeDetected = core.AIMeleeAttackDetector.GetEntityDetected();
        playerDirection = core.EntityDetector.CheckFlipDirectionTowardsEntity();
    }

    public override void Enter()
    {
        base.Enter();

        core.Movement.SetVelocityX(0f);
        isIdleTimeOver = false;

        if( idleTime == 0.0f)
        {
            SetRandomIdleTime();
        }
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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if(isPlayerDetected)
        {
            core.Movement.SetFlip(playerDirection);
        }
    }

    public void SetFlipAfterIdle(bool flip)
    {
        flipAfterIdle = flip;
    }

    public void SetRandomIdleTime()
    {
        idleTime = Random.Range(stateData.minIdleTime, stateData.maxIdleTime);
    }

    public void SetIdleTime(float idleTime)
    {
        this.idleTime = idleTime;
    }
}