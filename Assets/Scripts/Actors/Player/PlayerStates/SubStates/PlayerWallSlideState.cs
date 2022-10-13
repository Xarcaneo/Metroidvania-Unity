using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerTouchingWallState
{
    private bool canWallJumpCoyoteTime = true;

    public PlayerWallSlideState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Exit()
    {
        base.Exit();

        if (canWallJumpCoyoteTime) player.InAirState.StartWallJumpCoyoteTime();

        canWallJumpCoyoteTime = true;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            Movement?.SetVelocityY(-playerData.wallSlideVelocity);
        }
    }

    public bool DisableWallJumpCoyoteTime() => canWallJumpCoyoteTime = false;
}
