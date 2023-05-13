using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackState : AttackState
{
    protected D_RangedAttackState stateData;
    protected Transform attackPosition;

    protected GameObject projectile;
    protected Projectile projectileScript;

    public RangedAttackState(Entity entity, StateMachine stateMachine, string animBoolName, D_RangedAttackState stateData, Transform attackPosition) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
        this.attackPosition = attackPosition;
    }

    public override void AnimationActionTrigger()
    {
        projectile = GameObject.Instantiate(stateData.projectile, attackPosition.position, attackPosition.rotation);
        projectile.transform.localScale = entity.transform.localScale;

        projectileScript = projectile.GetComponent<Projectile>();

        projectileScript.FireProjectile(stateData.projectileSpeed,
            stateData.projectileTravelDistance, m_damageData, (int)entity.transform.localScale.x);
    }
}
