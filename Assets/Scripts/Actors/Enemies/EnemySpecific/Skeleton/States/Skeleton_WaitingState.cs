using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_WaitingState : WaitingState
{
    private readonly Skeleton enemy;

    public Skeleton_WaitingState(Entity entity, StateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
        this.enemy = (Skeleton)entity;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (playerPosition != Movement.FacingDirection)
            Movement.Flip();

        if (!isPlayerDetected)
        {
            Movement.Flip();
            stateMachine.ChangeState(enemy.patrolState);
        }
        else if (!isDetectingWall && isDetectingLedge)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
