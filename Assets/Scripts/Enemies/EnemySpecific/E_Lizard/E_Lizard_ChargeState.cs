using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Lizard_ChargeState : ChargeState
{
    private E_Lizard enemy;

    public E_Lizard_ChargeState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_ChargeState stateData, E_Lizard enemy) : base(entity, stateMachine, animBoolName, stateData)
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

        if (!isExitingState)
        {
            if (!isDectectingLedge || isDetectingWall)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
            else if (performCloseRangeAction)
            {
                stateMachine.ChangeState(enemy.meleeAttackState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}