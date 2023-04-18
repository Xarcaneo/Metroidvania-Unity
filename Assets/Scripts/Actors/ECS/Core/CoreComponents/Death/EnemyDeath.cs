using Opsive.UltimateInventorySystem.DropsAndPickups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : Death
{
    [SerializeField] private RandomItemDropper m_randomItemDropper;

    public override void Die()
    {
        base.Die();
        m_randomItemDropper.Drop();
    }
}
