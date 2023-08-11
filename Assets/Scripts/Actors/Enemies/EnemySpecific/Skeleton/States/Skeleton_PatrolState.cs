using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_PatrolState : MoveState
{
    private readonly Skeleton enemy;

    private bool isPlayerDetected;

    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    public Skeleton_PatrolState(Entity entity, StateMachine stateMachine, string animBoolName, D_MoveState stateData, Skeleton enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerDetected = EntityDetector.EntityInRange();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isPlayerDetected)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
        else if (isDetectingWall || !isDetectingLedge)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
