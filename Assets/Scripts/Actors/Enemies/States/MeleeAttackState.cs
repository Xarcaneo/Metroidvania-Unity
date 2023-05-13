using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : AttackState
{
    public MeleeAttackState(Entity entity, StateMachine stateMachine, string animBoolName, D_MeleeAttack stateData) : base(entity, stateMachine, animBoolName)
    {
    }

    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        //Checks what IDamageable entities intersects with weapon collider and damage them
        EnemyDamageHitBox?.MeleeAttack(m_damageData);
        EnemyDamageHitBox?.Knockback(Movement.FacingDirection);
    }
}