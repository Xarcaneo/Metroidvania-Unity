using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Opsive.UltimateInventorySystem.Equipping;
using EventHandler = Opsive.Shared.Events.EventHandler;
using Opsive.UltimateInventorySystem.Core;
using PixelCrushers;

public class Stats : CoreComponent
{
    [SerializeField] protected float m_BaseMaxHealth;
    [SerializeField] protected float m_BaseMaxMana;
    [SerializeField] protected int m_BaseAttack;
    [SerializeField] protected int m_BaseDefense;

    protected float maxHealth;
    protected float maxMana;
    protected int attack;
    protected int defense;

    protected float currentHealth;
    protected float currentMana;

    private Equipper m_Equipper;

    //Events
    public event Action HealthZero;
    public event Action StatsUpdated;
    public event Action<float> Damaged;
    public event Action<float> Healed;
    public event Action<float> ManaUsed;
    public event Action<float> ManaRestored;

    private bool saveDataApplied = false;

    protected override void Awake()
    {
        base.Awake();

        m_Equipper = transform.root.GetComponent<Equipper>();
   
        if (m_Equipper != null)
        {
            EventHandler.RegisterEvent(m_Equipper, EventNames.c_Equipper_OnChange, UpdateStats);
        }

        InitializeStats();
    }

    private void OnEnable() => SaveSystem.saveDataApplied += OnSaveDataApplied;
    private void OnDisable() => SaveSystem.saveDataApplied += OnSaveDataApplied;
    private void OnSaveDataApplied() => saveDataApplied = true;

    protected virtual void InitializeStats()
    {
        maxHealth = m_BaseMaxHealth;
        maxMana = m_BaseMaxMana;
        attack = m_BaseAttack;
        defense = m_BaseDefense;
        
        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    public void DecreaseHealth(float amount)
    {
        var damage = CalculateDamageReceived(amount);
  
        currentHealth -= damage;
        Damaged?.Invoke(damage);

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            HealthZero?.Invoke();
        }
    }

    public void IncreaseHealth(float amount)
    {
        Healed?.Invoke(amount);
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    public void DecreaseMana(float amount)
    {
        currentMana -= amount;
        ManaUsed?.Invoke(amount);
    }

    public void IncreaseMana(float amount)
    {
        ManaRestored?.Invoke(amount);
        currentMana = Mathf.Clamp(currentMana + amount, 0, maxMana);
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxMana()
    {
        return maxMana;
    }

    public int GetAttack()
    {
        return attack;
    }

    public int GetDefense()
    {
        return defense;
    }

    public float CalculateDamageReceived(float incomingDamage)
    {
        float damageReduction = Mathf.Clamp(defense, 0, 100);
        float damageReceived = incomingDamage * (1 - (damageReduction / 100f));
        return damageReceived;
    }

    protected virtual void UpdateStats()
    {
        maxHealth = m_BaseMaxHealth + m_Equipper.GetEquipmentStatInt("Health");
        maxMana = m_BaseMaxHealth + m_Equipper.GetEquipmentStatInt("Mana");
        attack = m_BaseAttack + m_Equipper.GetEquipmentStatInt("Attack");
        defense = m_BaseDefense + m_Equipper.GetEquipmentStatInt("Defense");
 
        StatsUpdated?.Invoke();

        if (!saveDataApplied) InitializeStats();
    }
}