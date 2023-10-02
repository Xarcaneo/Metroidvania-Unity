using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected int xInput;
    protected int yInput;

    private bool jumpInput;
    private bool attackInput;
    private bool blockInput;
    private bool isGrounded;
    protected bool isTouchingLadder;
    private bool dashInput;
    protected bool isOnSlope;
    protected bool isTouchingWall;

    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;
    protected CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    public PlayerGroundedState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isGrounded = CollisionSenses.Ground;
            isTouchingLadder = collisionSenses.Ladder;
            isOnSlope = CollisionSenses.SlopeCheck();
            isTouchingWall = CollisionSenses.WallFront;
        }
    }

    public override void Enter()
    {
        base.Enter();
        player.JumpState.ResetAmountOfJumpsLeft();
    }

    public override void Exit()
    {
        base.Exit();
        
        player.RigidBody2D.sharedMaterial = playerData.noFriction;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        jumpInput = player.InputHandler.JumpInput;
        dashInput = player.InputHandler.RollOrDashInput;
        attackInput = player.InputHandler.AttackInput;
        blockInput = player.InputHandler.BlockInput;

        if (isOnSlope)
        {
            Movement?.SetVelocityY(0.0f);;
            if (xInput == 0.0f || player.CrouchIdleState.isCrouching)
                player.RigidBody2D.sharedMaterial = playerData.fullFriction;
            else
                player.RigidBody2D.sharedMaterial = playerData.noFriction;
        }
        else
        {
            player.RigidBody2D.sharedMaterial = playerData.noFriction;
        }

        if (attackInput)
        {
            stateMachine.ChangeState(player.AttackState);
        }
        else if (blockInput)
        {
            stateMachine.ChangeState(player.PrepareBlockState);
        }
        else if (!isGrounded)
        {
            player.InAirState.StartCoyoteTime();
            stateMachine.ChangeState(player.InAirState);
        }
        else if (dashInput && player.RollState.CheckIfCanRoll())
        {
            stateMachine.ChangeState(player.RollState);
        }
        else if (jumpInput && player.JumpState.CanJump() && !player.CrouchIdleState.isCrouching && Movement?.CurrentVelocity.y == 0.00f)
        {
            stateMachine.ChangeState(player.JumpState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}