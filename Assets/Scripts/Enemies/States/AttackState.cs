using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    protected bool isEnemyInRangeDetected;

    public AttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isEnemyInRangeDetected = core.AIMeleeAttackDetector.GetEntityDetected();
    }

    public override void Enter()
    {
        base.Enter();

        core.Movement.SetVelocityX(0f);
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