using UnityEngine;

/// <summary>
/// Standard projectile implementation that follows a straight path and applies gravity after a certain distance.
/// Handles collision detection, damage dealing, and knockback effects.
/// </summary>
public class Projectile : ProjectileBase
{
    /// <summary>
    /// Updates the projectile's rotation based on its velocity when gravity is active.
    /// </summary>
    private void Update()
    {
        if (!hasHitGround && isGravityOn)
        {
            UpdateProjectileRotation();
        }
    }

    /// <summary>
    /// Handles physics-based updates including ground collision and gravity activation.
    /// </summary>
    private void FixedUpdate()
    {
        if (!hasHitGround)
        {
            CheckGroundCollision();
            CheckGravityActivation();
        }

        if (shouldDestroy)
        {
            animator.SetBool("Destroy", true);
        }
    }

    /// <summary>
    /// Handles collision with damageable and knockbackable entities.
    /// </summary>
    /// <param name="collision">The collider that triggered the collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasHitGround)
        {
            HandleDamageableCollision(collision);
            HandleKnockbackableCollision(collision);
        }
    }

    /// <summary>
    /// Updates the projectile's rotation to match its velocity direction.
    /// </summary>
    private void UpdateProjectileRotation()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;

        if (angle > 90 || angle < -90)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            angle += 180f;
        }
        else
        {
            transform.localScale = Vector3.one;
        }

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    /// <summary>
    /// Checks for collision with ground layers and stops the projectile if collision occurs.
    /// </summary>
    private void CheckGroundCollision()
    {
        if (!damagePosition) return;
        
        Collider2D groundHit = Physics2D.OverlapCircle(damagePosition.position, damageRadius, whatIsGround);
        if (groundHit)
        {
            StopProjectile();
        }
    }

    /// <summary>
    /// Checks if the projectile should activate gravity based on travel distance.
    /// </summary>
    private void CheckGravityActivation()
    {
        if (Vector2.Distance(startPos, transform.position) >= travelDistance && !isGravityOn)
        {
            ActivateGravity();
        }
    }

    /// <summary>
    /// Activates gravity effect on the projectile.
    /// </summary>
    private void ActivateGravity()
    {
        isGravityOn = true;
        rb.gravityScale = gravity;
    }

    /// <summary>
    /// Stops the projectile's movement and marks it for destruction.
    /// </summary>
    private void StopProjectile()
    {
        hasHitGround = true;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        shouldDestroy = true;
    }

    /// <summary>
    /// Handles collision with objects implementing IDamageable interface.
    /// </summary>
    /// <param name="collision">The collider to check for IDamageable interface.</param>
    private void HandleDamageableCollision(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damage(damageData);
            StopProjectile();
        }
    }

    /// <summary>
    /// Handles collision with objects implementing IKnockbackable interface.
    /// </summary>
    /// <param name="collision">The collider to check for IKnockbackable interface.</param>
    private void HandleKnockbackableCollision(Collider2D collision)
    {
        IKnockbackable knockbackable = collision.GetComponent<IKnockbackable>();
        if (knockbackable != null)
        {
            knockbackable.ReceiveKnockback(damageData, (int)direction.x);
            StopProjectile();
        }
    }

    /// <summary>
    /// Draws debug visualization for the damage radius in the editor.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (damagePosition != null)
        {
            Gizmos.DrawWireSphere(damagePosition.position, damageRadius);
        }
    }
}
