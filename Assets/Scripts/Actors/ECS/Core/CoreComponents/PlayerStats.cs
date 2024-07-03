using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats 
{
    private bool isNewSession = false;


    private void OnEnable() => GameEvents.Instance.onNewSession += OnNewSession;
    private void OnDisable() => GameEvents.Instance.onNewSession -= OnNewSession;

    private void OnNewSession() => isNewSession = true;


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
}
