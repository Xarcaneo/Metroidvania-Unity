using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrepareBlockState : PlayerAbilityState
{
    private Block Block { get => block ?? core.GetCoreComponent(ref block); }

    private Block block;

    private DamageReceiver DamageReceiver { get => damageReceiver ?? core.GetCoreComponent(ref damageReceiver); }

    private DamageReceiver damageReceiver;

    private bool successfulBlock = false;

    public PlayerPrepareBlockState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        DamageReceiver.OnSuccessfulBlock += OnSuccesfullBlock;

        player.InputHandler.UseBlockInput();
    }

    public override void Exit()
    {
        base.Exit();

        DamageReceiver.OnSuccessfulBlock -= OnSuccesfullBlock;

        successfulBlock = false;
        Block.isBlocking = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);

        if (successfulBlock)
        {
            stateMachine.ChangeState(player.BlockState);
        }
        else if (!isExitingState && isAnimationFinished)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    private void OnSuccesfullBlock()
    {
        successfulBlock = true;
    }

    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        if (!Block.isBlocking) Block.isBlocking = true;
        else Block.isBlocking = false;
    }

}
