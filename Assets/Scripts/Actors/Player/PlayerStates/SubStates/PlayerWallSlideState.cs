using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerTouchingWallState
{
    private bool canWallJumpCoyoteTime = true;

    public PlayerWallSlideState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if(Movement.FacingDirection == 1)
            player.SetColliderWidth(playerData.WallSlideColliderWidthRight);
        else
            player.SetColliderWidth(playerData.WallSlideColliderWidthLeft);
    }

    public override void Exit()
    {
        base.Exit();

        if (canWallJumpCoyoteTime) player.InAirState.StartWallJumpCoyoteTime();

        canWallJumpCoyoteTime = true;

        player.SetColliderWidth(playerData.WallSlideColliderWidthBase);
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
