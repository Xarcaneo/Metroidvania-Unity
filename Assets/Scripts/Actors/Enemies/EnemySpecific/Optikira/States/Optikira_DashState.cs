using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optikira_DashState : DashState
{
    private readonly Optikira enemy;
    private HurtArea HurtArea { get => hurtArea ?? core.GetCoreComponent(ref hurtArea); }
    private HurtArea hurtArea;

    public Optikira_DashState(Entity entity, StateMachine stateMachine, string animBoolName, D_DashState stateData) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = (Optikira)entity;
    }

    public override void Enter()
    {
        base.Enter();

        HurtArea.isActive = false;

        if (!DashBackCollision()) dashDirection = -Movement.FacingDirection;
        else dashDirection = Movement.FacingDirection;
    }

    public override void Exit()
    {
        base.Exit();

        HurtArea.isActive = true;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            Movement?.SetVelocity(stateData.dashVelocity, stateData.dashAngle, dashDirection);

            if (Time.time >= startTime + stateData.dashTime || Movement?.CurrentVelocity.x == 0)
            {
                if (isPlayerDetected)
                    stateMachine.ChangeState(enemy.rangedAttackState);
                else
                    stateMachine.ChangeState(enemy.idleState);
            }
        }
    }

}
