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
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (playerPosition != Movement.FacingDirection)
            Movement.Flip();

        if (!isPlayerDetected)
        {
            Movement.Flip();
            stateMachine.ChangeState(enemy.patrolState);
        }
        else if (attackableTargetDetected)
        {
            stateMachine.ChangeState(enemy.meleeAttackState);
        }
        else if(!isDetectingWall && isDetectingLedge)
        {
            stateMachine.ChangeState(enemy.chasteState);
        }
    }
}
