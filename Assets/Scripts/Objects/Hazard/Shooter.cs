using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    private GameObject projectile;
    private Projectile projectileScript;

    [SerializeField] private Animator m_anim;

    [SerializeField] private Transform attackPosition;
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileTravelDistance;
    [SerializeField] private float damage;

    [SerializeField] private bool horizontalProjectile;
    [SerializeField] private bool verticalProjectile;
    [SerializeField] private float shootCooldown = 1f;

    private float shootTimer = 0f;

    private IDamageable.DamageData m_damageData;

    private void Awake()
    {
        m_damageData.SetData(null, damage);
    }

    private void Update()
    {
        // Update the shoot timer
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0)
        {
            m_anim.SetBool("shoot", true);
            // Reset the shoot timer
            shootTimer = shootCooldown;
        }
    }

    public void AnimationActionTrigger()
    {
        projectile = GameObject.Instantiate(projectilePrefab, attackPosition.position, attackPosition.rotation);
        projectile.transform.localScale = this.transform.localScale;

        projectileScript = projectile.GetComponent<Projectile>();

        Vector2 projectileDirection = Vector2.zero;

        if (horizontalProjectile && verticalProjectile)
        {
            // Get player's position
            Transform playerTransform = Player.Instance.transform;
            Vector2 playerPosition = playerTransform.position;

            // Calculate direction from projectile to player
            projectileDirection = playerPosition - (Vector2)attackPosition.position;
        }
        else
        {
            if (horizontalProjectile)
                projectileDirection.x = -(int)this.transform.localScale.x;
            if (verticalProjectile)
                projectileDirection.y = -(int)this.transform.localScale.y;
        }

        projectileScript.FireProjectile(projectileSpeed,
            projectileTravelDistance, m_damageData, projectileDirection);
    }

    public void AnimationFinishTrigger() => m_anim.SetBool("shoot", false);
}
