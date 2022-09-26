using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckpointInteractionState : PlayerState
{
    private float checkpointPosX;
    public PlayerCheckpointInteractionState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        checkpointPosX = checkpointPosX + playerData.checkpointActivationOffset * player.Core.Movement.FacingDirection;

        player.transform.position = new Vector2(checkpointPosX, player.transform.position.y);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public void SetDetectedPosition(float pos) => checkpointPosX = pos;
}
