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

        damageEventHandler = (amount) => { stateMachine.ChangeState(enemy.hurtState); };
        healthZeroEventHandler = () => { stateMachine.ChangeState(enemy.deathState); };

        SubscribeEvents();
    }

    public override void Exit()
    {
        base.Exit();

        UnsubscribeEvents();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished && !isExitingState)
        {
            enemy.afterAttackState.SetStateDurationTime(stateData.attackSpeed);
            stateMachine.ChangeState(enemy.afterAttackState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
