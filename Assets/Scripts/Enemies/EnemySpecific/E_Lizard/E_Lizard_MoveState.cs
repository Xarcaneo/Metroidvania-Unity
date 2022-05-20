using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Lizard_MoveState : MoveState
{
    private E_Lizard enemy;

    public E_Lizard_MoveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, E_Lizard enemy) : base(entity, stateMachine, animBoolName, stateData)
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

        if (isDetectingWall || !isDetectingLedge)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
        else if(isPlayerDetected)
        {
            stateMachine.ChangeState(enemy.chargeState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}