using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Handles detection and application of damage and knockback to entities within a trigger area.
/// </summary>
public class DamageHitBox : CoreComponent
{
    #region Detected Entities
    private readonly List<IDamageable> detectedDamageables = new List<IDamageable>();
    private readonly List<IKnockbackable> detectedKnockbackables = new List<IKnockbackable>();
    #endregion

    #region Unity Trigger Events
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        // Check for damageable component
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null) 
        {
            detectedDamageables.Add(damageable);
        }

        // Check for knockbackable component
        IKnockbackable knockbackable = collision.GetComponent<IKnockbackable>();
        if (knockbackable != null) 
        {
            detectedKnockbackables.Add(knockbackable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == null) return;

        // Remove from damageable list
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null) 
        {
            detectedDamageables.Remove(damageable);
        }

        // Remove from knockbackable list
        IKnockbackable knockbackable = collision.GetComponent<IKnockbackable>();
        if (knockbackable != null) 
        {
            detectedKnockbackables.Remove(knockbackable);
        }
    }
    #endregion

    #region Combat Actions
    /// <summary>
    /// Applies melee damage to all detected damageable entities
    /// </summary>
    /// <param name="damageData">Data about the damage to apply</param>
    public void MeleeAttack(IDamageable.DamageData damageData)
    {
        foreach (IDamageable item in detectedDamageables.ToList())
        {
            if (item != null)
            {
                item.Damage(damageData);
            }
        }
    }

    /// <summary>
    /// Applies knockback to all detected knockbackable entities
    /// </summary>
    /// <param name="damageData">Data about the damage causing knockback</param>
    /// <param name="facingDirection">Direction the attacker is facing</param>
    public void Knockback(IDamageable.DamageData damageData, int facingDirection)
    {
        foreach (IKnockbackable item in detectedKnockbackables.ToList())
        {
            if (item != null)
            {
                item.ReceiveKnockback(damageData, facingDirection);
            }
        }
    }
    #endregion

    private void OnDisable()
    {
        // Clear lists when disabled to prevent stale references
        detectedDamageables.Clear();
        detectedKnockbackables.Clear();
    }
}
