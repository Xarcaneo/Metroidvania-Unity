using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optikira_AttackCooldownState : AttackCooldownState
{
    private readonly Optikira enemy;

    private bool isPlayerDetected;

    private EntityDetector EntityDetector { get => entityDetector ?? core.GetCoreComponent(ref entityDetector); }
    private EntityDetector entityDetector;

    public Optikira_AttackCooldownState(Entity entity, StateMachine stateMachine, string animBoolName, D_AttackCooldownState stateData) : base(entity, stateMachine, animBoolName, stateData)
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

        if (EntityDetector.entityToRight != Movement.FacingDirection)
            Movement.Flip();

        if (timeSinceEnteredState >= stateData.cooldownTime)
        {
            if (isPlayerDetected)
                stateMachine.ChangeState(enemy.rangedAttackState);
            else
                stateMachine.ChangeState(enemy.idleState);
        }
    }
}
