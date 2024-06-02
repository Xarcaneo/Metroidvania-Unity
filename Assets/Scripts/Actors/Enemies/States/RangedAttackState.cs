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

        Vector2 projectileDirection = Vector2.zero;

        if (stateData.horizontalProjectile && stateData.verticalProjectile)
        {
            // Get player's position
            Transform playerTransform = Player.Instance.transform;
            Vector2 playerPosition = playerTransform.position;

            // Calculate direction from projectile to player
            projectileDirection = playerPosition - (Vector2)attackPosition.position;
        }
        else
        {
            if (stateData.horizontalProjectile)
                projectileDirection.x = (int)entity.transform.localScale.x;
            if (stateData.verticalProjectile)
                projectileDirection.y = (int)entity.transform.localScale.y;
        }

        projectileScript.FireProjectile(stateData.projectileSpeed,
            stateData.projectileTravelDistance, m_damageData, projectileDirection);
    }
}
