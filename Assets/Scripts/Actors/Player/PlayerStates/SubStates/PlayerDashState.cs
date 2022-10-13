using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerAbilityState
{
    private float lastDashTime;

    private Vector2 dashDirection;

    public PlayerDashState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();

        player.InputHandler.UseDashInput();

        dashDirection = Vector2.right * Movement.FacingDirection;

        startTime = Time.unscaledTime;

        float angle = Vector2.SignedAngle(Vector2.right, dashDirection);

        startTime = Time.time;
        Movement?.CheckIfShouldFlip(Mathf.RoundToInt(dashDirection.x));
    }

    public override void Exit()
    {
        base.Exit();

        if (Movement?.CurrentVelocity.y > 0)
        {
            Movement?.SetVelocityY(Movement.CurrentVelocity.y * playerData.dashEndYMultiplier);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            player.Anim.SetFloat("yVelocity", Movement.CurrentVelocity.y);

            Movement?.SetVelocity(playerData.dashVelocity, playerData.dashAngle, Movement.FacingDirection);

            if (Time.time >= startTime + playerData.dashTime || isTouchingWall  || isTouchingLedge || Movement?.CurrentVelocity.x == 0)
            {
                isAbilityDone = true;
                lastDashTime = Time.time;
            }
        }
    }

    public bool CheckIfCanDash()
    {
        return Time.time >= lastDashTime + playerData.dashCooldown;
    }
}