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
    #region Fields

    /// <summary>
    /// Flag indicating if this is a new game session.
    /// </summary>
    private bool isNewSession = false;

    #endregion

    #region Unity Event Methods

    /// <summary>
    /// Subscribes to the new session event when the component is enabled.
    /// </summary>
    private void OnEnable() => GameEvents.Instance.onNewSession += OnNewSession;

    /// <summary>
    /// Unsubscribes from the new session event when the component is disabled.
    /// </summary>
    private void OnDisable() => GameEvents.Instance.onNewSession -= OnNewSession;

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles the new session event by setting the new session flag.
    /// </summary>
    private void OnNewSession() => isNewSession = true;

    #endregion

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

        if (isNewSession)
        {
            currentHealth = maxHealth;
            currentMana = maxMana;

            isNewSession = false;
        }
    }

    #endregion
}
