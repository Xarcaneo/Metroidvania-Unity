using UnityEngine;

/// <summary>
/// Base class for all projectiles in the game. Provides common functionality and fields
/// that all projectiles share, such as movement parameters and basic components.
/// </summary>
public abstract class ProjectileBase : MonoBehaviour
{
    [Header("Core Settings")]
    /// <summary>
    /// Gravity scale applied to the projectile after reaching travel distance.
    /// </summary>
    [SerializeField] protected float gravity = 1f;

    /// <summary>
    /// Radius for ground collision detection and damage application.
    /// </summary>
    [SerializeField] protected float damageRadius = 0.5f;

    /// <summary>
    /// Transform used for damage and collision detection position.
    /// </summary>
    [SerializeField] protected Transform damagePosition;

    /// <summary>
    /// LayerMask defining what layers are considered ground for collision.
    /// </summary>
    [SerializeField] protected LayerMask whatIsGround;

    /// <summary>
    /// LayerMask defining what layers are considered player for collision.
    /// </summary>
    [SerializeField] protected LayerMask whatIsPlayer;

    /// <summary>
    /// Data containing damage information to apply on hit.
    /// </summary>
    protected IDamageable.DamageData damageData;

    /// <summary>
    /// Movement speed of the projectile.
    /// </summary>
    protected float speed;

    /// <summary>
    /// Distance the projectile travels before gravity is applied.
    /// </summary>
    protected float travelDistance;

    /// <summary>
    /// Direction the projectile is moving in.
    /// </summary>
    protected Vector2 direction;

    /// <summary>
    /// Starting position of the projectile.
    /// </summary>
    protected Vector2 startPos;

    /// <summary>
    /// Reference to the Rigidbody2D component.
    /// </summary>
    protected Rigidbody2D rb;

    /// <summary>
    /// Reference to the Animator component.
    /// </summary>
    protected Animator animator;

    /// <summary>
    /// Whether gravity has been activated for this projectile.
    /// </summary>
    protected bool isGravityOn;

    /// <summary>
    /// Whether the projectile has hit the ground.
    /// </summary>
    protected bool hasHitGround;

    /// <summary>
    /// Whether the projectile should be destroyed.
    /// </summary>
    protected bool shouldDestroy;

    /// <summary>
    /// Initializes components and sets up initial movement parameters.
    /// </summary>
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0.0f;
        rb.velocity = direction * speed;
        isGravityOn = false;
        startPos = transform.position;
    }

    /// <summary>
    /// Initializes the projectile with movement parameters and damage data.
    /// </summary>
    /// <param name="speed">Initial speed of the projectile.</param>
    /// <param name="travelDistance">Distance before gravity is applied.</param>
    /// <param name="damageData">Damage data to apply on hit.</param>
    /// <param name="direction">Direction to fire the projectile in.</param>
    public virtual void FireProjectile(float speed, float travelDistance, IDamageable.DamageData damageData, Vector2 direction)
    {
        this.speed = speed;
        this.travelDistance = travelDistance;
        this.damageData = damageData;
        this.direction = direction.normalized;
    }

    /// <summary>
    /// Called by animation events to destroy the projectile GameObject.
    /// </summary>
    public virtual void AnimationFinishTrigger()
    {
        Destroy(gameObject);
    }
}
