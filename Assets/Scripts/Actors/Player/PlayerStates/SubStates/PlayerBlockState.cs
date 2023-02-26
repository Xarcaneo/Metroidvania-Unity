using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockState : PlayerAbilityState
{
    protected Block Block { get => block ?? core.GetCoreComponent(ref block); }

    private Block block;

    public PlayerBlockState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        Movement?.SetVelocityX(0f);

        player.InputHandler.UseBlockInput();
    }

    public override void Exit()
    {
        base.Exit();

        Block.isBlocking = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);

        if (!isExitingState && isAnimationFinished)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        if (!Block.isBlocking) Block.isBlocking = true;
        else Block.isBlocking = false;
    }

}
