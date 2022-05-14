using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeState : State
{
    protected D_ChargeState stateData;

    protected bool isPlayerDetected;
    protected bool isDectectingLedge;
    protected bool isDetectingWall;

    protected bool performLongRangeAction;
    protected bool performCloseRangeAction;

    protected int playerDirection;

    public ChargeState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_ChargeState stateData) : base(etity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerDetected = core.EntityDetector.GetEntityDetected();
        isDectectingLedge = core.CollisionSenses.LedgeVertical;
        isDetectingWall = core.CollisionSenses.WallFront;
        playerDirection = core.EntityDetector.CheckFlipDirectionTowardsEntity();
        performCloseRangeAction = core.AIMeleeAttackDetector.GetEntityDetected();
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