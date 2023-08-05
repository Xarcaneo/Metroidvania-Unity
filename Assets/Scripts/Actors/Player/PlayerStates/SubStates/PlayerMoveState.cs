using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
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

        Movement?.CheckIfShouldFlip(xInput);

        if (!isExitingState)
        {
            if (!isOnSlope)
                Movement?.SetVelocityX(playerData.movementVelocity * xInput);
            else if (isOnSlope && !isTouchingWall)
                Movement?.SetVelocityXOnSlope(playerData.movementVelocity * -xInput);

            if (xInput == 0)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else if (yInput == -1)
            {
                stateMachine.ChangeState(player.CrouchIdleState);
            }
        }
    }
}