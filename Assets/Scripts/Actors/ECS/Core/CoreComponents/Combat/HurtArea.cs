using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtArea : CoreComponent
{
    [SerializeField] private LayerMask targetLayer;
    private Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;

    public bool isActive = true;

    private IDamageable.DamageData m_damageData;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive && (targetLayer.value & 1 << other.gameObject.layer) != 0)
        {
            m_damageData.SetData(core.Parent, Stats.GetAttack());

            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null) damageable.Damage(m_damageData);

            IKnockbackable knockbackable = other.GetComponent<IKnockbackable>();
            if (knockbackable != null) knockbackable.ReceiveKnockback();
        }
    }
}