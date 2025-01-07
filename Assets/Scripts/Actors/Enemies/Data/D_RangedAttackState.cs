using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data container for enemy ranged attack state configuration.
/// Handles projectile properties and attack behavior.
/// </summary>
[CreateAssetMenu(fileName = "newRangedAttackData", menuName = "Data/State Data/RangedAttack State")]
public class D_RangedAttackState : D_BaseState
{
    [Header("Projectile Settings")]
    [Tooltip("Prefab of the projectile to spawn")]
    [SerializeField]
    public GameObject projectile;

    [Tooltip("Damage dealt by the projectile")]
    [Min(0)]
    public float projectileDamage = 10f;

    [Tooltip("Speed of the projectile")]
    [Min(0)]
    public float projectileSpeed = 12f;

    [Tooltip("Maximum distance the projectile can travel")]
    [Min(0)]
    public float projectileTravelDistance = 10f;

    [Header("Attack Direction")]
    [Tooltip("Whether the projectile can be fired vertically")]
    public bool verticalProjectile;

    [Tooltip("Whether the projectile can be fired horizontally")]
    public bool horizontalProjectile = true;

    private void OnValidate()
    {
        // Validate numeric values
        projectileDamage = Mathf.Max(0f, projectileDamage);
        projectileSpeed = Mathf.Max(0f, projectileSpeed);
        projectileTravelDistance = Mathf.Max(0f, projectileTravelDistance);

        // Ensure at least one direction is enabled
        if (!verticalProjectile && !horizontalProjectile)
        {
            horizontalProjectile = true;
        }

        // Warn if projectile prefab is missing
        if (projectile == null)
        {
            Debug.LogWarning($"Projectile prefab is missing in {name}");
        }
    }
}
