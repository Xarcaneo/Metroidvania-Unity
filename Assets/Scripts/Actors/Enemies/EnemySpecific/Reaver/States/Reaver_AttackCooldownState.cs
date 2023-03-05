using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaver_AttackCooldownState : AttackCooldownState
{
    private Reaver enemy;

    private bool attackableTargetDetected;

    private EnemyWeapon EnemyWeapon { get => enemyWeapon ?? core.GetCoreComponent(ref enemyWeapon); }
    private EnemyWeapon enemyWeapon;

    public Reaver_AttackCooldownState(Entity entity, StateMachine stateMachine, string animBoolName, D_AttackCooldownState stateData, Reaver enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        attackableTargetDetected = EnemyWeapon.EntityInRange();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (timeSinceEnteredState >= stateData.cooldownTime)
        {
            if (attackableTargetDetected)
                stateMachine.ChangeState(enemy.meleeAttackState);
            else
                stateMachine.ChangeState(enemy.moveState);
        }
    }
}
