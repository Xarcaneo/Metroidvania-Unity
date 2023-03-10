using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrepareBlockState : PlayerAbilityState
{
    private Block Block { get => block ?? core.GetCoreComponent(ref block); }

    private Block block;

    private Combat Combat { get => combat ?? core.GetCoreComponent(ref combat); }

    private Combat combat;

    private bool successfulBlock = false;

    public PlayerPrepareBlockState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        Combat.OnSuccessfulBlock += OnSuccesfullBlock;

        Movement?.SetVelocityX(0f);

        player.InputHandler.UseBlockInput();
    }

    public override void Exit()
    {
        base.Exit();

        Combat.OnSuccessfulBlock -= OnSuccesfullBlock;

        successfulBlock = false;
        Block.isBlocking = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(successfulBlock)
        {
            stateMachine.ChangeState(player.BlockState);
        }
        else if (!isExitingState && isAnimationFinished)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    private void OnSuccesfullBlock() => successfulBlock = true;

    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        if (!Block.isBlocking) Block.isBlocking = true;
        else Block.isBlocking = false;
    }

}
