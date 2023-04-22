using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrepareClimb : PlayerState
{
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    public PlayerPrepareClimb(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);

        if (isAnimationFinished)
        {
            stateMachine.ChangeState(player.LadderClimbState);
        }
    }
}
