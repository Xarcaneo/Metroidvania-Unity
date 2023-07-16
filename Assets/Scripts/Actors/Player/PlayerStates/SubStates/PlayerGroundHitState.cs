using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundHitState : PlayerState
{
    public PlayerGroundHitState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if( isAnimationFinished )
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
