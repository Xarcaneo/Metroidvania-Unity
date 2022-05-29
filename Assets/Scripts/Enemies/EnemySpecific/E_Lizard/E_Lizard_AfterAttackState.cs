using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Lizard_AfterAttackState : AfterAttackState
{
    private E_Lizard enemy;

    public E_Lizard_AfterAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, E_Lizard enemy) : base(entity, stateMachine, animBoolName)
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

        core.Movement.SetFlip(playerDirection);

        if (isStateTimeOver && !isExitingState)
        {
            if(isEnemyInRangeDetected && isPlayerInSight)
            {
                stateMachine.ChangeState(enemy.meleeAttackState);
            }
            else if (isPlayerDetected && isDectectingLedge && !isEnemyInRangeDetected)
            {
                stateMachine.ChangeState(enemy.chargeState);
            }
            else if (!isPlayerDetected)
            {
                stateMachine.ChangeState(enemy.moveState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
