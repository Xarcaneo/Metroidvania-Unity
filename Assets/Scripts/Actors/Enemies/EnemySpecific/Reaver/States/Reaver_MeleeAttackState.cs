using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaver_MeleeAttackState : MeleeAttackState
{
    private Reaver enemy;

    public Reaver_MeleeAttackState(Entity entity, StateMachine stateMachine, string animBoolName, D_MeleeAttack stateData, Reaver enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
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

        if (isAnimationFinished && !isExitingState)
        {
            stateMachine.ChangeState(enemy.attackCooldownState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}