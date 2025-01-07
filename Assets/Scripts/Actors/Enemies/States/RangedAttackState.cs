using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a ranged attack state, where the entity fires a projectile towards the target.
/// </summary>
public class RangedAttackState : AttackState
{
    /// <summary>
    /// The data associated with the ranged attack state, containing parameters like projectile type, speed, and travel distance.
    /// </summary>
    protected D_RangedAttackState stateData;

    /// <summary>
    /// The position from which the projectile is fired.
    /// </summary>
    protected Transform attackPosition;

    /// <summary>
    /// The projectile GameObject to be instantiated and fired.
    /// </summary>
    protected GameObject projectile;

    /// <summary>
    /// The script attached to the projectile for controlling its behavior.
    /// </summary>
    protected Projectile projectileScript;

    /// <summary>
    /// Initializes a new instance of the <see cref="RangedAttackState"/> class.
    /// </summary>
    /// <param name="entity">The entity associated with the state.</param>
    /// <param name="stateMachine">The state machine controlling the entity's states.</param>
    /// <param name="animBoolName">The name of the animation boolean associated with this state.</param>
    /// <param name="stateData">The data containing ranged attack parameters like projectile type, speed, and travel distance.</param>
    /// <param name="attackPosition">The position from which the projectile is fired.</param>
    public RangedAttackState(Entity entity, StateMachine stateMachine, string animBoolName, D_RangedAttackState stateData, Transform attackPosition) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
        this.attackPosition = attackPosition;
    }

    /// <summary>
    /// Triggered when the animation action occurs for the ranged attack. Instantiates and fires the projectile.
    /// </summary>
    public override void AnimationActionTrigger()
    {
        // Instantiate the projectile at the attack position and set its scale to match the entity's scale
        projectile = GameObject.Instantiate(stateData.projectile, attackPosition.position, attackPosition.rotation);
        projectile.transform.localScale = entity.transform.localScale;

        // Get the projectile script to control the projectile behavior
        projectileScript = projectile.GetComponent<Projectile>();

        if (projectileScript == null)
        {
            Debug.LogError("Projectile script missing on instantiated projectile.");
            return;
        }

        // Determine the direction in which to fire the projectile
        Vector2 projectileDirection;

        if (stateData.horizontalProjectile && stateData.verticalProjectile)
        {
            // Get player's position if both horizontal and vertical projectiles are enabled
            Transform playerTransform = Player.Instance.transform;
            Vector2 playerPosition = playerTransform.position;

            // Calculate direction from projectile to player
            projectileDirection = playerPosition - (Vector2)attackPosition.position;
        }
        else
        {
            // For horizontal and/or vertical projectile, determine direction based on entity's facing
            projectileDirection = new Vector2(stateData.horizontalProjectile ? (int)entity.transform.localScale.x : 0,
                                              stateData.verticalProjectile ? (int)entity.transform.localScale.y : 0);
        }

        // Fire the projectile with the calculated direction
        projectileScript.FireProjectile(stateData.projectileSpeed,
            stateData.projectileTravelDistance, m_damageData, projectileDirection);
    }
}
