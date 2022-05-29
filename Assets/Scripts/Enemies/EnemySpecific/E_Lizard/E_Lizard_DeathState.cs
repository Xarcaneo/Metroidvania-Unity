using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Lizard_DeathState : DeathState
{
    private E_Lizard enemy;

    public E_Lizard_DeathState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, E_Lizard enemy) : base(entity, stateMachine, animBoolName)
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

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
