using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected int xInput;
    protected int yInput;
    protected bool useHotbarItemInput;
    private bool jumpInput;
    private bool attackInput;
    private bool blockInput;
    private bool actionInput;

    private bool isGrounded;
    protected bool isTouchingLadder;
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
        actionInput = player.InputHandler.ActionInput;
        attackInput = player.InputHandler.AttackInput;
        blockInput = player.InputHandler.BlockInput;
        useHotbarItemInput = player.InputHandler.HotbarActionInput;

        if (isOnSlope)
        {
            if (xInput == 0.0f || player.CrouchIdleState.isCrouching)
                player.RigidBody2D.sharedMaterial = playerData.fullFriction;
            else
                player.RigidBody2D.sharedMaterial = playerData.noFriction;

            Movement?.SetVelocityY(0.0f);
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
        else if (useHotbarItemInput && !Menu.GameMenu.Instance.gameHotbar.IsSlotEmpty())
        {
            stateMachine.ChangeState(player.UseHotbarItem);
        }
        else if (!isGrounded)
        {
            player.InAirState.StartCoyoteTime();
            stateMachine.ChangeState(player.InAirState);
        }
        else if (actionInput && player.RollState.CheckIfCanRoll())
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