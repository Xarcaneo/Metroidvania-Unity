using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnedState : PlayerState
{
    private bool isGrounded;

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    public PlayerSpawnedState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isGrounded = CollisionSenses.Ground;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isGrounded && Movement?.CurrentVelocity.y < 0.01f)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
