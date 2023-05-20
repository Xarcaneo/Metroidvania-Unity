using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optikira_IdleState : IdleState
{
    private readonly Optikira enemy;

    private bool isPlayerDetected;

    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    public Optikira_IdleState(Entity entity, StateMachine stateMachine, string animBoolName, D_IdleState stateData, Optikira enemy) : base(entity, stateMachine, animBoolName, stateData)
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

        if (isIdleTimeOver)
        {
            flipAfterIdle = true;
            stateMachine.ChangeState(enemy.patrolState);
        }
        else if (isPlayerDetected)
        {
            stateMachine.ChangeState(enemy.rangedAttackState);
        }
    }
}
