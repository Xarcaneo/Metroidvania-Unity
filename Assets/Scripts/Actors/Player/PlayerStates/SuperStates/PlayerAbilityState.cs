using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityState : PlayerState
{
    //Variable to check if ability is done
    protected bool isAbilityDone;

    //Variables to check player's current state
    private bool isGrounded;
    protected bool isTouchingWall;
    protected bool isTouchingLedge;

    //References to Movement and CollisionSenses components
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    protected CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    //Constructor to initialize variables
    public PlayerAbilityState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    //Checks player's current state
    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isGrounded = CollisionSenses.Ground;
            isTouchingWall = CollisionSenses.WallFront;
            isTouchingLedge = CollisionSenses.LedgeHorizontal;
        }
    }

    //Resets ability state when entered
    public override void Enter()
    {
        base.Enter();

        isAbilityDone = false;
    }

    //Checks conditions to change state
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAbilityDone)
        {
            if (isGrounded && Movement?.CurrentVelocity.y < 0.01f)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else if (isTouchingWall)
            {
                player.WallSlideState.DisableWallJumpCoyoteTime();
                stateMachine.ChangeState(player.WallSlideState);
            }
            else if (isTouchingWall && !isTouchingLedge && !isGrounded)
            {
                stateMachine.ChangeState(player.LedgeClimbState);
            }
            else
            {
                stateMachine.ChangeState(player.InAirState);
            }
        }
    }
}