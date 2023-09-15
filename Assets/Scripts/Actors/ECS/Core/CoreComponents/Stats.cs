using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Opsive.UltimateInventorySystem.Equipping;
using EventHandler = Opsive.Shared.Events.EventHandler;
using Opsive.UltimateInventorySystem.Core;

public class Stats : CoreComponent
{
    [SerializeField] private float m_BaseMaxHealth;
    [SerializeField] private int m_BaseAttack;
    [SerializeField] private int m_BaseDefense;

    private float maxHealth;
    private int attack;
    private int defense;

    private float currentHealth;

    private Equipper m_Equipper;

    //Events
    public event Action HealthZero;
    public event Action StatsUpdated;
    public event Action<float> Damaged;

    protected override void Awake()
    {
        base.Awake();

        maxHealth = m_BaseMaxHealth;
        attack = m_BaseAttack;
        defense = m_BaseDefense;

        currentHealth = maxHealth;

        m_Equipper = transform.root.GetComponent<Equipper>();

        if (m_Equipper != null)
        {
            EventHandler.RegisterEvent(m_Equipper, EventNames.c_Equipper_OnChange, UpdateStats);
            UpdateStats();
        }
    }

    public void DecreaseHealth(float amount)
    {
        currentHealth -= amount;
        Damaged?.Invoke(amount);

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            HealthZero?.Invoke();
        }
    }

    public void IncreaseHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetAttack()
    {
        return attack;
    }

    public int GetDefense()
    {
        return defense;
    }

    public void UpdateStats()
    {
        maxHealth = m_BaseMaxHealth + m_Equipper.GetEquipmentStatInt("MaxHealth");
        attack = m_BaseAttack + m_Equipper.GetEquipmentStatInt("Attack");
        defense = m_BaseDefense + m_Equipper.GetEquipmentStatInt("Defense");

        StatsUpdated?.Invoke();
    }
}