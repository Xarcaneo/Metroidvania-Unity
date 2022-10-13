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
    protected bool isEnemyInAttackRangeDetected;
    protected bool isPlayerInSight;

    protected float idleTime;

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

    public IdleState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData) : base(etity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isDetectingWall = CollisionSenses.WallFront;
            isDectectingLedge = CollisionSenses.LedgeVertical;
        }

        if (EntityDetector)
        {
            playerDirection = EntityDetector.CheckFlipDirectionTowardsEntity();
            isPlayerDetected = EntityDetector.GetEntityDetected();
        }

        if (AIMeleeAttackDetector)
        {
            isEnemyInAttackRangeDetected = AIMeleeAttackDetector.GetEntityDetected();
        }

        if (aIRaycast)
        {
            isPlayerInSight = AIRaycast.CheckRaycastCollision();
        }
    }

    public override void Enter()
    {
        base.Enter();

        Movement?.SetVelocityX(0f);
        isIdleTimeOver = false;
        SetRandomIdleTime();
    }

    public override void Exit()
    {
        base.Exit();

        if (flipAfterIdle)
        {
            Movement?.Flip();
        }

        idleTime = 0.0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);

        if (Time.time >= startTime + idleTime)
        {
            isIdleTimeOver = true;
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