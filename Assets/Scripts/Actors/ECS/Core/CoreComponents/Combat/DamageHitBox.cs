using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DamageHitBox : CoreComponent
{
    private List<IDamageable> detectedDamageables = new List<IDamageable>(); // list of damageables detected within range
    private List<IKnockbackable> detectedKnockbackables = new List<IKnockbackable>(); // list of knockbackables detected within range

    void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null) detectedDamageables.Add(damageable);

        IKnockbackable knockbackable = collision.GetComponent<IKnockbackable>();
        if (knockbackable != null) detectedKnockbackables.Add(knockbackable);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null) detectedDamageables.Remove(damageable);

        IKnockbackable knockbackable = collision.GetComponent<IKnockbackable>();
        if (knockbackable != null) detectedKnockbackables.Remove(knockbackable);
    }

    public void MeleeAttack(IDamageable.DamageData damageData)
    {
        foreach (IDamageable item in detectedDamageables.ToList())
        {
            item.Damage(damageData);
        }
    }    

    public void Knockback(int facingDirection)
    {
        // loop through all detected knockbackables and apply knockback to each one
        foreach (IKnockbackable item in detectedKnockbackables.ToList())
        {
            item.ReceiveKnockback(facingDirection);
        }
    }
}
