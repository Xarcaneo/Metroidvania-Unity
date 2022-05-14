using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Lizard_MeleeAttackState : MeleeAttackState
{
    private E_Lizard enemy;

    public E_Lizard_MeleeAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_MeleeAttack stateData, E_Lizard enemy) : base(entity, stateMachine, animBoolName, stateData)
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
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FinishAttack()
    {
        base.FinishAttack();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            enemy.idleState.SetFlipAfterIdle(false);
            enemy.idleState.SetIdleTime(stateData.attackSpeed);
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();
    }
}
