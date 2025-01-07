using Opsive.UltimateInventorySystem.DropsAndPickups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles enemy-specific death behavior and effects.
/// </summary>
public class EnemyDeath : Death
{
    #region Settings

    /// <summary>
    /// The random item dropper to use when the enemy dies.
    /// </summary>
    [SerializeField, Tooltip("Random item dropper")]
    private RandomItemDropper m_randomItemDropper;

    /// <summary>
    /// The amount of souls to award when this enemy dies.
    /// </summary>
    [SerializeField, Tooltip("Souls awarded when killed")]
    public int souls;

    #endregion

    #region Death Implementation

    /// <summary>
    /// Implements enemy-specific death behavior.
    /// Drops items and destroys the enemy.
    /// </summary>
    public override void Die()
    {
        base.Die();
        m_randomItemDropper?.Drop();
    }

    #endregion
}
