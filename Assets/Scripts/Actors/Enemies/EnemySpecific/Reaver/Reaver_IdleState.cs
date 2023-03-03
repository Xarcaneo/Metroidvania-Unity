using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaver_IdleState : IdleState
{
    private Reaver enemy;
    public Reaver_IdleState(Entity entity, StateMachine stateMachine, string animBoolName, D_IdleState stateData, Reaver enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
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

        if (isIdleTimeOver)
        {
            flipAfterIdle = true;
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
