using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Lizard_PlayerDetectedState : PlayerDetectedState
{
    private E_Lizard enemy;

    public E_Lizard_PlayerDetectedState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, E_Lizard enemy) : base(etity, stateMachine, animBoolName, stateData)
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

        if (performLongRangeAction)
        {
            stateMachine.ChangeState(enemy.chargeState);
        }
        else if (!isPlayerInMaxAgroRange)
        {
            stateMachine.ChangeState(enemy.lookForPlayerState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}