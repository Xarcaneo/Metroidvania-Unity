using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static IDamageable;

public class Weapon : CoreComponent
{
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    private Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;

    private List<IDamageable> detectedDamageables = new List<IDamageable>(); // list of damageables detected within range
    private List<IKnockbackable> detectedKnockbackables = new List<IKnockbackable>(); // list of knockbackables detected within range

    private DamageData damageData; // data for damage dealt by this weapon

    public void AddToDetected(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
        {
            detectedDamageables.Add(damageable);
        }

        IKnockbackable knockbackable = collision.GetComponent<IKnockbackable>();

        if (knockbackable != null)
        {
            detectedKnockbackables.Add(knockbackable);
        }
    }

    public void RemoveFromDetected(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
        {
            detectedDamageables.Remove(damageable);
        }

        IKnockbackable knockbackable = collision.GetComponent<IKnockbackable>();

        if (knockbackable != null)
        {
            detectedKnockbackables.Remove(knockbackable);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        AddToDetected(collision);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        RemoveFromDetected(collision);
    }

    public void CheckMeleeAttack()
    {
        // loop through all detected damageables and deal damage to each one
        foreach (IDamageable item in detectedDamageables.ToList())
        {
            damageData.SetData(core.Parent, Stats.GetAttack());
            item.Damage(damageData);
        }

        // loop through all detected knockbackables and apply knockback to each one
        foreach (IKnockbackable item in detectedKnockbackables.ToList())
        {
            item.Knockback(Movement.FacingDirection);
        }
    }
}
