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

        dashDirection = Vector2.right * core.Movement.FacingDirection;

        startTime = Time.unscaledTime;

        float angle = Vector2.SignedAngle(Vector2.right, dashDirection);

        startTime = Time.time;
        core.Movement.CheckIfShouldFlip(Mathf.RoundToInt(dashDirection.x));
    }

    public override void Exit()
    {
        base.Exit();

        if (core.Movement.CurrentVelocity.y > 0)
        {
            core.Movement.SetVelocityY(core.Movement.CurrentVelocity.y * playerData.dashEndYMultiplier);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            player.Anim.SetFloat("yVelocity", core.Movement.CurrentVelocity.y);

            core.Movement.SetVelocity(playerData.dashVelocity, playerData.dashAngle, core.Movement.FacingDirection);

            if (Time.time >= startTime + playerData.dashTime || isTouchingWall  || isTouchingLedge || core.Movement.CurrentVelocity.x == 0)
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