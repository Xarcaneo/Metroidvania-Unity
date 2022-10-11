using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouchingWallState : PlayerState
{
    private float touchingWallTime = 0.0f;
    private bool canWallJump = false;

    protected bool isGrounded;
    protected bool isTouchingWall;
    protected bool jumpInput;
    protected int xInput;
    protected int yInput;

    public PlayerTouchingWallState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = core.CollisionSenses.Ground;
        isTouchingWall = core.CollisionSenses.WallFront;
    }

    public override void Enter()
    {
        base.Enter();

        touchingWallTime = Time.time;

        damageEventHandler = (amount) => { stateMachine.ChangeState(player.HurtState); };
        healthZeroEventHandler = () => { stateMachine.ChangeState(player.DeathState); };

        SubscribeEvents();
    }

    public override void Exit()
    {
        base.Exit();

        UnsubscribeEvents();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        jumpInput = player.InputHandler.JumpInput;

        CheckIfCanWallJump();

        if (!isTouchingWall || xInput != core.Movement.FacingDirection)
        {
            stateMachine.ChangeState(player.InAirState);
        }
        else if (jumpInput && canWallJump)
        {
            player.WallJumpState.DetermineWallJumpDirection(isTouchingWall);
            stateMachine.ChangeState(player.WallJumpState);
        }
        else if (isGrounded)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    private void CheckIfCanWallJump()
    {
        if (Time.time > touchingWallTime + playerData.wallTouchTime) canWallJump = true;
        else canWallJump = false;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}