using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Opsive.UltimateInventorySystem.Equipping;
using EventHandler = Opsive.Shared.Events.EventHandler;
using Opsive.UltimateInventorySystem.Core;
using PixelCrushers;

/// <summary>
/// Base class for managing entity statistics including health, mana, attack, and defense.
/// Handles stat calculations, equipment modifiers, and related events.
/// </summary>
public class Stats : CoreComponent
{
    #region Serialized Fields

    [SerializeField] protected float m_BaseMaxHealth;
    [SerializeField] protected float m_BaseMaxMana;
    [SerializeField] protected int m_BaseAttack;
    [SerializeField] protected int m_BaseDefense;

    #endregion

    #region Protected Fields

    protected float maxHealth;
    protected float maxMana;
    protected int attack;
    protected int defense;

    protected float currentHealth;
    protected float currentMana;

    #endregion

    #region Private Fields

    private Equipper m_Equipper;
    private StatUpgradeManager m_StatUpgradeManager;

    #endregion

    #region Events

    /// <summary>
    /// Triggered when health reaches zero.
    /// </summary>
    public event Action HealthZero;

    /// <summary>
    /// Triggered when stats are updated (e.g., through equipment changes).
    /// </summary>
    public event Action StatsUpdated;

    /// <summary>
    /// Triggered when the entity takes damage.
    /// </summary>
    public event Action<float> Damaged;

    /// <summary>
    /// Triggered when the entity is healed.
    /// </summary>
    public event Action<float> Healed;

    /// <summary>
    /// Triggered when mana is consumed.
    /// </summary>
    public event Action<float> ManaUsed;

    /// <summary>
    /// Triggered when mana is restored.
    /// </summary>
    public event Action<float> ManaRestored;

    #endregion

    #region Unity Callback Methods

    /// <summary>
    /// Initializes the component and sets up equipment event handling.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        m_Equipper = transform.root.GetComponent<Equipper>();
        m_StatUpgradeManager = GetComponent<StatUpgradeManager>();

        if (m_Equipper != null)
        {
            EventHandler.RegisterEvent(m_Equipper, EventNames.c_Equipper_OnChange, UpdateStats);
        }
        else
        {
            InitializeStats();
        }
    }

    #endregion

    #region Stats Management

    /// <summary>
    /// Initializes base stats with default values.
    /// </summary>
    protected virtual void InitializeStats()
    {
        // Apply base stats with upgrade multiplier
        float upgradeMultiplier = m_StatUpgradeManager != null ? m_StatUpgradeManager.CurrentBonusMultiplier : 1f;
        maxHealth = m_BaseMaxHealth * upgradeMultiplier;
        maxMana = m_BaseMaxMana * upgradeMultiplier;
        attack = Mathf.RoundToInt(m_BaseAttack * upgradeMultiplier);
        defense = Mathf.RoundToInt(m_BaseDefense * upgradeMultiplier);
        
        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    /// <summary>
    /// Decreases health by the specified amount, accounting for defense.
    /// </summary>
    /// <param name="amount">Amount of damage to apply</param>
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

    /// <summary>
    /// Increases health by the specified amount, capped at max health.
    /// </summary>
    /// <param name="amount">Amount of healing to apply</param>
    public void IncreaseHealth(float amount)
    {
        Healed?.Invoke(amount);
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    /// <summary>
    /// Sets health to a specific value.
    /// </summary>
    /// <param name="amount">New health value</param>
    public void SetHealth(float amount) => currentHealth = amount;

    /// <summary>
    /// Sets mana to a specific value.
    /// </summary>
    /// <param name="amount">New mana value</param>
    public void SetMana(float amount) => currentMana = amount;

    /// <summary>
    /// Decreases mana by the specified amount.
    /// </summary>
    /// <param name="amount">Amount of mana to consume</param>
    public void DecreaseMana(float amount)
    {
        currentMana -= amount;
        ManaUsed?.Invoke(amount);
    }

    /// <summary>
    /// Increases mana by the specified amount, capped at max mana.
    /// </summary>
    /// <param name="amount">Amount of mana to restore</param>
    public void IncreaseMana(float amount)
    {
        ManaRestored?.Invoke(amount);
        currentMana = Mathf.Clamp(currentMana + amount, 0, maxMana);
    }

    #endregion

    #region Stat Getters

    /// <summary>
    /// Gets the maximum health value.
    /// </summary>
    public float GetMaxHealth() => maxHealth;

    /// <summary>
    /// Gets the current health value.
    /// </summary>
    public float GetCurrentHealth() => currentHealth;

    /// <summary>
    /// Gets the maximum mana value.
    /// </summary>
    public float GetMaxMana() => maxMana;

    /// <summary>
    /// Gets the current mana value.
    /// </summary>
    public float GetCurrentMana() => currentMana;

    /// <summary>
    /// Gets the attack value.
    /// </summary>
    public int GetAttack() => attack;

    /// <summary>
    /// Gets the defense value.
    /// </summary>
    public int GetDefense() => defense;

    #endregion

    #region Calculations

    /// <summary>
    /// Calculates the actual damage received after applying defense.
    /// </summary>
    /// <param name="incomingDamage">Raw incoming damage</param>
    /// <returns>Actual damage after defense calculation</returns>
    public float CalculateDamageReceived(float incomingDamage)
    {
        float damageReduction = Mathf.Clamp(defense, 0, 100);
        float damageReceived = incomingDamage * (1 - (damageReduction / 100f));
        return damageReceived;
    }

    /// <summary>
    /// Updates stats based on equipment modifiers.
    /// </summary>
    public virtual void UpdateStats()
    {
        if (!m_Equipper) return;
        InitializeStats();

        // Add equipment bonuses
        maxHealth += m_Equipper.GetEquipmentStatInt("Health");
        maxMana += m_Equipper.GetEquipmentStatInt("Mana");
        attack += m_Equipper.GetEquipmentStatInt("Attack");
        defense += m_Equipper.GetEquipmentStatInt("Defense");

        StatsUpdated?.Invoke();
    }

    #endregion
}