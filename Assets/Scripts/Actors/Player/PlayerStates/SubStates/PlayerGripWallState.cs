using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGripWallState : PlayerState
{
    private Vector2 gripPos;
    private float touchingWallTime = 0.0f;
    private bool canWallJump = false;
    private bool isTouchingWall;

    private bool jumpInput;
    private int yInput;

    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    protected CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }

    private Movement movement;
    private CollisionSenses collisionSenses;

    public PlayerGripWallState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isTouchingWall = CollisionSenses.WallFront;
        }
    }

    public override void Enter()
    {
        base.Enter();

        touchingWallTime = Time.time;
        gripPos = player.transform.position;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        CheckIfCanWallJump();

        yInput = player.InputHandler.NormInputY;
        jumpInput = player.InputHandler.JumpInput;

        if (!isExitingState)
        {
            Movement?.SetVelocityZero();
            player.transform.position = gripPos;
        }

        if (isAnimationFinished)
        {
            if (jumpInput && canWallJump)
            {
                player.WallJumpState.DetermineWallJumpDirection(isTouchingWall);
                stateMachine.ChangeState(player.WallJumpState);
            }
            else if (yInput == -1)
            {
                stateMachine.ChangeState(player.InAirState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        player.InputHandler.UseBlockInput();
    }

    private void CheckIfCanWallJump()
    {
        if (Time.time > touchingWallTime + playerData.wallTouchTime) canWallJump = true;
        else canWallJump = false;
    }
}
