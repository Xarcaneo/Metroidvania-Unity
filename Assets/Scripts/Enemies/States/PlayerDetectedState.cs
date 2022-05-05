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

        isPlayerDetected = entity.playerDetector.GetPlayerDetected();
        playerDirection = entity.playerDetector.CheckFlipDirectionTowardsPlayer();
    }

    public override void Enter()
    {
        base.Enter();

        performLongRangeAction = false;
        entity.SetVelocity(0f);
        DoChecks();
        entity.SetFacingTowardsPlayer(playerDirection);
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