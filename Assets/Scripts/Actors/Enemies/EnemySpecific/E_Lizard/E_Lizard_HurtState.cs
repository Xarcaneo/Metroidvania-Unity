using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Lizard_HurtState : HurtState
{
    private E_Lizard enemy;

    public E_Lizard_HurtState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, E_Lizard enemy) : base(entity, stateMachine, animBoolName)
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

        if (isAnimationFinished)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
