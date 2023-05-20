using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrepareClimb : PlayerState
{
    private int climbingDirection;

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    public PlayerPrepareClimb(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Exit()
    {
        base.Exit();

        if (climbingDirection == 1)
            Player.Instance.gameObject.transform.position += new Vector3(0, 1f, 0);
        else if (climbingDirection == -1)
            Player.Instance.gameObject.transform.position -= new Vector3(0, 1f, 0);

        climbingDirection = 0;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);
        Movement?.SetVelocityY(0f);

        if (isAnimationFinished)
        {
            stateMachine.ChangeState(player.LadderClimbState);
        }
    }

    public void SetClimbingDirection(int direciton) => climbingDirection = direciton;
}
