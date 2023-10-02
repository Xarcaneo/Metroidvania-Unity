using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_MeleeAttackState : AttackState
{
    private readonly Skeleton enemy;

    private bool attackableTargetDetected;

    public Skeleton_MeleeAttackState(Entity entity, StateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
        this.enemy = (Skeleton)entity;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        attackableTargetDetected = EnemyDamageHitBox.EntityInRange();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        EnemyDamageHitBox?.MeleeAttack(m_damageData);
        EnemyDamageHitBox?.Knockback(m_damageData, Movement.FacingDirection);

        if (Movement.FacingDirection != EnemyDamageHitBox.entityToRight)
        {
            Movement.Flip();
        }

        if (!attackableTargetDetected)
        {
            stateMachine.ChangeState(enemy.patrolState);
        }
    }
}
