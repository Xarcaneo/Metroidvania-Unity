using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    protected bool isEnemyInRangeDetected;

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;


    public AttackState(Entity entity, StateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
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

        Movement?.SetVelocityX(0f);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}