using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_IdleState : IdleState
{
    private readonly Skeleton enemy;

    private bool isPlayerDetected;

    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    public Skeleton_IdleState(Entity etity, StateMachine stateMachine, string animBoolName, D_IdleState stateData, Skeleton enemy) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerDetected = EntityDetector.EntityInRange();
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isIdleTimeOver)
        {
            flipAfterIdle = true;
            stateMachine.ChangeState(enemy.patrolState);
        }
        else if (isPlayerDetected)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
