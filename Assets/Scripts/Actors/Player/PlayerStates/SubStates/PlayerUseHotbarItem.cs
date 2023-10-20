using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUseHotbarItem : PlayerState
{
    public PlayerUseHotbarItem(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState && isAnimationFinished)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        Menu.GameMenu.Instance.gameHotbar.UseItem();
    }
}
