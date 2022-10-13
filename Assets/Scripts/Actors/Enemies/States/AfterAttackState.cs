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

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private AIMeleeAttackDetector AIMeleeAttackDetector { get => aIMeleeAttackDetector ?? core.GetCoreComponent(ref aIMeleeAttackDetector); }
    private AIRaycast AIRaycast { get => aIRaycast ?? core.GetCoreComponent(ref aIRaycast); }

    private Movement movement;
    private CollisionSenses collisionSenses;
    private EntityDetector entityDetector;
    private AIMeleeAttackDetector aIMeleeAttackDetector;
    private AIRaycast aIRaycast;

    public AfterAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isDectectingLedge = CollisionSenses.LedgeVertical;
        }

        if (EntityDetector)
        {
            isPlayerDetected = EntityDetector.GetEntityDetected();
            playerDirection = EntityDetector.CheckFlipDirectionTowardsEntity();
        }

        if (aIMeleeAttackDetector)
        {
            isEnemyInRangeDetected = AIMeleeAttackDetector.GetEntityDetected();
        }

        if (AIRaycast)
        {
            isPlayerInSight = AIRaycast.CheckRaycastCollision();
        }
    }

    public override void Enter()
    {
        base.Enter();
        Movement?.SetVelocityX(0f);
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

        Movement?.SetVelocityX(0f);

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
