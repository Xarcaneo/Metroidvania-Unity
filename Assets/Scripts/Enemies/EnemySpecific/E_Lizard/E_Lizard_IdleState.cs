 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Lizard_IdleState : IdleState
{
    private E_Lizard enemy;
    public E_Lizard_IdleState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, E_Lizard enemy) : base(entity, stateMachine, animBoolName, stateData)
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
            if (!isPlayerDetected)
            {
                if (!isDectectingLedge && !flipAfterIdle)
                {
                    core.Movement.Flip();
                }

                stateMachine.ChangeState(enemy.moveState);
            }
            else if (isEnemyInRangeDetected && isIdleTimeOver)
            {
                stateMachine.ChangeState(enemy.meleeAttackState);
            }
            else if (isPlayerDetected && isDectectingLedge && !isEnemyInRangeDetected)
            {
                stateMachine.ChangeState(enemy.chargeState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
