using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Weapon : MonoBehaviour
{
    private List<IDamageable> detectedDamageables = new List<IDamageable>();

    public void AddToDetected(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
        {
            detectedDamageables.Add(damageable);
        }
    }

    public void RemoveFromDetected(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
        {
            detectedDamageables.Remove(damageable);
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
        foreach (IDamageable item in detectedDamageables.ToList())
        {
            item.Damage(10);
        }
    }
}
