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

        if (isIdleTimeOver)
        {
            if (!isPlayerDetected || isPlayerDetected && !isPlayerInSight)
            {
                flipAfterIdle = true;
                stateMachine.ChangeState(enemy.moveState);
            }
        }

        if (isPlayerInSight)
        {
            if (isEnemyInAttackRangeDetected)
            {
                flipAfterIdle = false;
                stateMachine.ChangeState(enemy.meleeAttackState);
            }
            else if (isPlayerDetected && isDectectingLedge && !isDetectingWall)
            {
                flipAfterIdle = false;
                stateMachine.ChangeState(enemy.chargeState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
