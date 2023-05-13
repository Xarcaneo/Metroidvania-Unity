using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaver_AttackCooldownState : AttackCooldownState
{
    private readonly Reaver enemy;

    private bool attackableTargetDetected;

    private EnemyDamageHitBox EnemyDamageHitBox { get => enemyDamageHitBox ?? core.GetCoreComponent(ref enemyDamageHitBox); }
    private EnemyDamageHitBox enemyDamageHitBox;

    public Reaver_AttackCooldownState(Entity entity, StateMachine stateMachine, string animBoolName, D_AttackCooldownState stateData) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = (Reaver)entity;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        attackableTargetDetected = EnemyDamageHitBox.EntityInRange();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (timeSinceEnteredState >= stateData.cooldownTime)
        {
            if (attackableTargetDetected)
                stateMachine.ChangeState(enemy.meleeAttackState);
            else
                stateMachine.ChangeState(enemy.patrolState);
        }
    }
}
