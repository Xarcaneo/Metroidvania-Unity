using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optikira_PatrolState : MoveState
{
    private Optikira enemy;

    private bool isPlayerDetected;

    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    public Optikira_PatrolState(Entity entity, StateMachine stateMachine, string animBoolName, D_MoveState stateData) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = (Optikira)entity;
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
            stateMachine.ChangeState(enemy.rangedAttackState);
        }
        else if (isDetectingWall || !isDetectingLedge)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
