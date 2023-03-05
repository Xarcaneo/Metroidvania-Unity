using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingState : State
{
    protected bool isPlayerDetected;

    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    protected EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    public WaitingState(Entity entity, StateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }
    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerDetected = EntityDetector.EntityInRange();
    }

    public override void Enter()
    {
        base.Enter();

        Movement?.SetVelocityX(0f);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
