using UnityEngine;

/// <summary>
/// Represents an area that can damage and apply knockback to entities that enter it.
/// </summary>
public class HurtArea : CoreComponent
{
    #region Settings
    [Header("Target Settings")]
    [SerializeField, Tooltip("Layer mask for entities that can be damaged by this area")]
    private LayerMask targetLayer;

    /// <summary>
    /// Controls whether the hurt area is currently active and can deal damage
    /// </summary>
    public bool isActive = true;
    #endregion

    #region Core Component References
    private Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;
    #endregion

    private IDamageable.DamageData m_damageData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive || other == null || Stats == null) return;

        // Check if the colliding object is on the target layer
        if ((targetLayer.value & 1 << other.gameObject.layer) == 0) return;

        // Set up damage data
        m_damageData.SetData(core.Parent, Stats.GetAttack());

        // Apply damage if possible
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damage(m_damageData);
        }

        // Apply knockback if possible
        IKnockbackable knockbackable = other.GetComponent<IKnockbackable>();
        if (knockbackable != null)
        {
            knockbackable.ReceiveKnockback();
        }
    }

    /// <summary>
    /// Sets the active state of the hurt area
    /// </summary>
    /// <param name="active">Whether the area should be active</param>
    public void SetActive(bool active)
    {
        isActive = active;
    }
}