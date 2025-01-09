using UnityEngine;

/// <summary>
/// A hazard that shoots projectiles at regular intervals.
/// Can shoot horizontally, vertically, or target the player directly.
/// </summary>
public class Shooter : MonoBehaviour
{
    [Header("References")]
    /// <summary>
    /// Reference to the shooter's animator component.
    /// </summary>
    [SerializeField] private Animator animator;

    /// <summary>
    /// Transform marking the position where projectiles spawn.
    /// </summary>
    [SerializeField] private Transform attackPosition;

    /// <summary>
    /// Prefab of the projectile to shoot.
    /// </summary>
    [SerializeField] private GameObject projectilePrefab;

    [Header("Projectile Settings")]
    /// <summary>
    /// Speed at which projectiles travel.
    /// </summary>
    [SerializeField] private float projectileSpeed = 10f;

    /// <summary>
    /// Distance projectiles travel before gravity affects them.
    /// </summary>
    [SerializeField] private float projectileTravelDistance = 5f;

    /// <summary>
    /// Amount of damage each projectile deals.
    /// </summary>
    [SerializeField] private float damage = 1f;

    [Header("Behavior Settings")]
    /// <summary>
    /// Whether the shooter fires horizontally.
    /// </summary>
    [SerializeField] private bool horizontalProjectile;

    /// <summary>
    /// Whether the shooter fires vertically.
    /// </summary>
    [SerializeField] private bool verticalProjectile;

    /// <summary>
    /// Time between shots in seconds.
    /// </summary>
    [SerializeField] private float shootCooldown = 1f;

    // Private fields
    private float shootTimer = 0f;
    private IDamageable.DamageData damageData;
    private GameObject currentProjectile;
    private Projectile currentProjectileScript;

    /// <summary>
    /// Initializes damage data on awake.
    /// </summary>
    private void Awake()
    {
        damageData.SetData(null, damage);
    }

    /// <summary>
    /// Updates the shoot timer and triggers shooting animation.
    /// </summary>
    private void Update()
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0f)
        {
            if (animator != null)
            {
                animator.SetBool("shoot", true);
            }
            shootTimer = shootCooldown;
        }
    }

    /// <summary>
    /// Called by animation to spawn and fire a projectile.
    /// </summary>
    public void AnimationActionTrigger()
    {
        if (projectilePrefab == null || attackPosition == null) return;

        // Spawn projectile
        currentProjectile = Instantiate(projectilePrefab, attackPosition.position, attackPosition.rotation);
        currentProjectile.transform.localScale = transform.localScale;

        currentProjectileScript = currentProjectile.GetComponent<Projectile>();
        if (currentProjectileScript == null) return;

        // Calculate projectile direction
        Vector2 projectileDirection = CalculateProjectileDirection();

        // Fire projectile
        currentProjectileScript.FireProjectile(projectileSpeed, projectileTravelDistance, damageData, projectileDirection);
    }

    /// <summary>
    /// Calculates the direction for the projectile based on shooter settings.
    /// </summary>
    /// <returns>Normalized direction vector for the projectile.</returns>
    private Vector2 CalculateProjectileDirection()
    {
        Vector2 direction = Vector2.zero;

        if (horizontalProjectile && verticalProjectile)
        {
            // Target player directly
            Transform playerTransform = Player.Instance?.transform;
            if (playerTransform != null)
            {
                direction = ((Vector2)playerTransform.position - (Vector2)attackPosition.position).normalized;
            }
        }
        else
        {
            // Use preset direction
            if (horizontalProjectile)
                direction.x = -(int)transform.localScale.x;
            if (verticalProjectile)
                direction.y = -(int)transform.localScale.y;
        }

        return direction;
    }

    /// <summary>
    /// Called by animation to reset the shooting state.
    /// </summary>
    public void AnimationFinishTrigger()
    {
        if (animator != null)
        {
            animator.SetBool("shoot", false);
        }
    }
}
