using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectedState : State
{
    protected D_PlayerDetected stateData;

    protected bool isPlayerDetected;
    protected bool performLongRangeAction;
    protected int playerDirection;

    public PlayerDetectedState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData) : base(etity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerDetected = core.EntityDetector.GetEntityDetected();
        playerDirection = core.EntityDetector.CheckFlipDirectionTowardsEntity();
    }

    public override void Enter()
    {
        base.Enter();

        performLongRangeAction = false;
        core.Movement.SetVelocityX(0f);
        DoChecks();
        core.Movement.SetFlip(playerDirection);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= startTime + stateData.longRangeActionTime)
        {
            performLongRangeAction = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}