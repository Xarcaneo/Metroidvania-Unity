using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    protected D_MoveState stateData;

    protected bool isDetectingWall;
    protected bool isDetectingLedge;
    protected bool isPlayerDetected;
    protected bool isPlayerInSight;

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private AIRaycast AIRaycast { get => aIRaycast ?? core.GetCoreComponent(ref aIRaycast); }

    private Movement movement;
    private CollisionSenses collisionSenses;
    private EntityDetector entityDetector;
    private AIRaycast aIRaycast;

    public MoveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (EntityDetector)
        {
            isPlayerDetected = EntityDetector.GetEntityDetected();
        }

        if (AIRaycast)
        {
            isPlayerInSight = AIRaycast.CheckRaycastCollision();
        }

        if (CollisionSenses)
        {
            isDetectingLedge = CollisionSenses.LedgeVertical;
            isDetectingWall = CollisionSenses.WallFront;
        }
    }

    public override void Enter()
    {
        base.Enter();

        Movement?.SetVelocityX(stateData.movementSpeed * Movement.FacingDirection);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
 
        Movement?.SetVelocityX(stateData.movementSpeed * Movement.FacingDirection);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

    }
}
