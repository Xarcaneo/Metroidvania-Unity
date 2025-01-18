using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages player-specific statistics and handles new game session initialization.
/// Inherits from the base Stats class and adds session management functionality.
/// </summary>
public class PlayerStats : Stats 
{

    #region Stats Initialization

    /// <summary>
    /// Initializes player stats with base values.
    /// If this is a new session, also resets current health and mana to maximum values.
    /// </summary>
    protected override void InitializeStats()
    {
        maxHealth = m_BaseMaxHealth;
        maxMana = m_BaseMaxMana;
        attack = m_BaseAttack;
        defense = m_BaseDefense;

        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
            currentMana = maxMana;
        }
    }
    #endregion
}
