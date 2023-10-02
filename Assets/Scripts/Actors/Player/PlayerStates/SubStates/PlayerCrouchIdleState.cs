using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchIdleState : PlayerGroundedState
{
    public bool isCrouching = false;

    public PlayerCrouchIdleState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        isCrouching = true;
        player.SetColliderHeight(playerData.rollColliderHeight);
    }

    public override void Exit()
    {
        base.Exit();

        isCrouching = false;
        player.SetColliderHeight(playerData.standColliderHeight);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);

        if (!isExitingState)
        {
            if (yInput != -1)
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }
}