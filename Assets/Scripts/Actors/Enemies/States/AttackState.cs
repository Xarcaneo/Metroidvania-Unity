using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    protected bool isEnemyInRangeDetected;

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private AIMeleeAttackDetector AIMeleeAttackDetector { get => aIMeleeAttackDetector ?? core.GetCoreComponent(ref aIMeleeAttackDetector); }

    private Movement movement;
    private AIMeleeAttackDetector aIMeleeAttackDetector;

    public AttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (AIMeleeAttackDetector)
        {
            isEnemyInRangeDetected = AIMeleeAttackDetector.GetEntityDetected();
        }
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