using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUseHotbarItem : PlayerState
{
    protected bool isOnSlope;
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    public PlayerUseHotbarItem(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isOnSlope = CollisionSenses.SlopeCheck();
        }
    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);

        if (isOnSlope)
        {
            player.RigidBody2D.sharedMaterial = playerData.fullFriction;
        }
        else
        {
            player.RigidBody2D.sharedMaterial = playerData.noFriction;
        }

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
