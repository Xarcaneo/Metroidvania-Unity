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

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private AIMeleeAttackDetector AIMeleeAttackDetector { get => aIMeleeAttackDetector ?? core.GetCoreComponent(ref aIMeleeAttackDetector); }

    private Movement movement;
    private CollisionSenses collisionSenses;
    private EntityDetector entityDetector;
    private AIMeleeAttackDetector aIMeleeAttackDetector;

    public ChargeState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_ChargeState stateData) : base(etity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isDectectingLedge = CollisionSenses.LedgeVertical;
            isDetectingWall = CollisionSenses.WallFront;
        }

        if (EntityDetector)
        {
            isPlayerDetected = EntityDetector.GetEntityDetected();
            playerDirection = EntityDetector.CheckFlipDirectionTowardsEntity();
        }

        if (AIMeleeAttackDetector)
        {
            performCloseRangeAction = AIMeleeAttackDetector.GetEntityDetected();
        }
    }

    public override void Enter()
    {
        base.Enter();
        DoChecks();
        Movement?.Flip(playerDirection);
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

        Movement?.Flip(playerDirection);

        if (isDectectingLedge && !isDetectingWall)
        {
            Movement?.SetVelocityX(stateData.chargeSpeed * Movement.FacingDirection);
        }
    }
}