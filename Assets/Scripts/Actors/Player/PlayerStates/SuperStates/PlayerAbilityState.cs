using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityState : PlayerState
{
    protected bool isAbilityDone;

    //Checks
    private bool isGrounded;
    protected bool isTouchingWall;
    protected bool isTouchingLedge;

    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    protected CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }

    private Movement movement;
    private CollisionSenses collisionSenses;

    public PlayerAbilityState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

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

    public override void Enter()
    {
        base.Enter();

        isAbilityDone = false;

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

        //Check conditions to change state
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