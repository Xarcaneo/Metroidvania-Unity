using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //private AttackDetails attackDetails;
    private IDamageable.DamageData damageData;

    private float speed;
    private int direction;
    private float travelDistance;
    private float xStartPos;

    [SerializeField]
    private float gravity;
    [SerializeField]
    private float damageRadius;

    private Rigidbody2D rb;

    private bool isGravityOn;
    private bool hasHitGround;

    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private LayerMask whatIsPlayer;
    [SerializeField]
    private Transform damagePosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0.0f;
        rb.velocity = transform.right * speed;

        isGravityOn = false;

        xStartPos = transform.position.x;
    }

    private void Update()
    {
        if (!hasHitGround)
        {
            //attackDetails.position = transform.position;

            if (isGravityOn)
            {
                float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
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
            }


            if (Mathf.Abs(xStartPos - transform.position.x) >= travelDistance && !isGravityOn)
            {
                isGravityOn = true;
                rb.gravityScale = gravity;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasHitGround)
        {
            var should_destroy = false;

            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(damageData);
                should_destroy = true;
            }

            IKnockbackable knockbackable = collision.GetComponent<IKnockbackable>();
            if (knockbackable != null)
            {
                knockbackable.ReceiveKnockback(direction);
                should_destroy = true;
            }

            if (should_destroy)
                Destroy(gameObject);
        }
    }

    public void FireProjectile(float speed, float travelDistance, IDamageable.DamageData damageData, int direction)
    {
        this.speed = speed * direction;
        this.direction = direction;
        this.travelDistance = travelDistance;
        this.damageData = damageData;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(damagePosition.position, damageRadius);
    }
}