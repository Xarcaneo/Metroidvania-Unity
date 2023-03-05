using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaver_MoveState : MoveState
{
    private Reaver enemy;

    private bool isPlayerDetected;

    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    public Reaver_MoveState(Entity entity, StateMachine stateMachine, string animBoolName, D_MoveState stateData, Reaver enemy) : base(entity, stateMachine, animBoolName, stateData)
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

        if(isPlayerDetected)
        {
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
        else if (isDetectingWall || !isDetectingLedge)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
