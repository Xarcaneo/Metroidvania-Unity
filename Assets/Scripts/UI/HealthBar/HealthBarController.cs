using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    protected float health;
    protected float lerpTimer;
    protected float maxHealth = 100;
    public float chipSpeed = 2f;

    public Image frontHealthBar;
    public Image backHealthBar;

    public Stats stats;

    virtual public void OnDestroy()
    {
        if (stats)
        {
            stats.Damaged -= TakeDamage;
        }
    }

    private void Update()
    {
        if (stats)
        {
            health = Mathf.Clamp(health, 0, maxHealth);
            UpdateHealthUI();
        }
    }

    virtual public void UpdateHealthUI()
    {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;

        if(fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.white;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }

        if(fillF < hFraction)
        {
            backHealthBar.color = Color.red;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
    }

    virtual public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }

    protected void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
    }
}
