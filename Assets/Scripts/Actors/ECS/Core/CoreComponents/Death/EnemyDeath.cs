using Opsive.UltimateInventorySystem.DropsAndPickups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : Death
{
    [SerializeField] private RandomItemDropper m_randomItemDropper;
    [SerializeField] public int souls;

    public override void Die()
    {
        base.Die();
        m_randomItemDropper.Drop();
    }
}
