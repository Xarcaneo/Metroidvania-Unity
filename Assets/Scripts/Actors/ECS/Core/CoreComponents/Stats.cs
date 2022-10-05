using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Stats : CoreComponent
{
    [SerializeField] private float maxHealth;
    private float currentHealth;

    //Events
    public event Action HealthZero;
    public event Action<float> Damaged;

    protected override void Awake()
    {
        base.Awake();

        currentHealth = maxHealth;
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
}