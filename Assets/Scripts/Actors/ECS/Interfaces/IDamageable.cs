using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Damage(DamageData damageData);
    void InstantKill();

    public struct DamageData
    {
        public float DamageAmount;
        public Entity Source;
        public bool CanBlock;

        public void SetData(Entity source, float damageAmount)
        {
            DamageAmount = damageAmount;
            Source = source;
        }
    }
}
