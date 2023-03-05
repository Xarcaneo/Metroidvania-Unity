using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaver_WaitingState : WaitingState
{
    private Reaver enemy;

    public Reaver_WaitingState(Entity entity, StateMachine stateMachine, string animBoolName, Reaver enemy) : base(entity, stateMachine, animBoolName)
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

        Movement.Flip();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isPlayerDetected)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}
