using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerAbilityState
{
    private float lastDashTime;

    private Vector2 dashDirection;

    public PlayerDashState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();

        player.InputHandler.UseDashInput();

        dashDirection = Vector2.right * Movement.FacingDirection;

        startTime = Time.time;
        Movement?.CheckIfShouldFlip(Mathf.RoundToInt(dashDirection.x));
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            if (Time.time >= startTime + playerData.dashTime || isTouchingWall || isOnSlope && isGrounded || isTouchingLedge)
            {
                isAbilityDone = true;
                lastDashTime = Time.time;
                Movement?.SetVelocityZero();
            }
            else
                Movement?.SetVelocity(playerData.dashVelocity, playerData.dashAngle, Movement.FacingDirection);
        }
    }

    public bool CheckIfCanDash()
    {
        return Time.time >= lastDashTime + playerData.dashCooldown;
    }
}