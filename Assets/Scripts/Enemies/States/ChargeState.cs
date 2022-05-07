using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeState : State
{
    protected D_ChargeState stateData;

    protected bool isPlayerDetected;
    protected bool isDectectingLedge;
    protected bool isDetectingWall;

    protected int playerDirection;

    public ChargeState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_ChargeState stateData) : base(etity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerDetected = entity.playerDetector.GetEntityDetected();
        isDectectingLedge = core.CollisionSenses.LedgeVertical;
        isDetectingWall = core.CollisionSenses.WallFront;
        playerDirection = entity.playerDetector.CheckFlipDirectionTowardsPlayer();
    }

    public override void Enter()
    {
        base.Enter();
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
        DoChecks();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        core.Movement.SetFlip(playerDirection);

        if (isDectectingLedge && !isDetectingWall)
        {
            core.Movement.SetVelocityX(stateData.chargeSpeed * core.Movement.FacingDirection);
        }
    }
}