using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //private AttackDetails attackDetails;
    private IDamageable.DamageData damageData;

    private float speed;
    private Vector2 direction;
    private float travelDistance;
    private Vector2 startPos;

    [SerializeField]
    private float gravity;
    [SerializeField]
    private float damageRadius;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isGravityOn;
    private bool hasHitGround;
    private bool should_destroy = false;

    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private LayerMask whatIsPlayer;
    [SerializeField]
    private Transform damagePosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();

        rb.gravityScale = 0.0f;
        rb.velocity = direction * speed;

        isGravityOn = false;

        startPos = transform.position;
    }

    private void Update()
    {
        if (!hasHitGround)
        {
            if (isGravityOn)
            {
                // Calculate the angle based on the velocity direction
                float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;

                // Rotate the projectile sprite
                if (angle > 90 || angle < -90)
                {
                    transform.localScale = new Vector3(-1f, 1f, 1f); // Flip sprite horizontally
                    angle += 180f;
                }
                else
                {
                    transform.localScale = new Vector3(1f, 1f, 1f); // Reset sprite scale
                }

                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!hasHitGround)
        {
            Collider2D groundHit = Physics2D.OverlapCircle(damagePosition.position, damageRadius, whatIsGround);

            if (groundHit)
            {
                hasHitGround = true;
                rb.gravityScale = 0f;
                rb.velocity = Vector2.zero;
                should_destroy = true;
            }

            if (Vector2.Distance(startPos, transform.position) >= travelDistance && !isGravityOn)
            {
                isGravityOn = true;
                rb.gravityScale = gravity;
            }
        }

        if (should_destroy)
            anim.SetBool("Destroy", true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasHitGround)
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(damageData);
                should_destroy = true;
                rb.gravityScale = 0f;
                rb.velocity = Vector2.zero;
            }

            IKnockbackable knockbackable = collision.GetComponent<IKnockbackable>();
            if (knockbackable != null)
            {
                knockbackable.ReceiveKnockback(damageData, (int)direction.x);
                should_destroy = true;
                rb.gravityScale = 0f;
                rb.velocity = Vector2.zero;
            }
        }
    }

    public void FireProjectile(float speed, float travelDistance, IDamageable.DamageData damageData, Vector2 direction)
    {
        this.speed = speed;
        this.direction = direction.normalized;
        this.travelDistance = travelDistance;
        this.damageData = damageData;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(damagePosition.position, damageRadius);
    }

    public void AnimationFinishTrigger() => Destroy(gameObject);
}
