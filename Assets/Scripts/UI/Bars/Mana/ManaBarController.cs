using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBarController : MonoBehaviour
{
    protected float mana;
    protected float lerpTimer;
    protected float maxMana = 100;
    public float chipSpeed = 2f;

    public Image frontManaBar;
    public Image backManaBar;

    public Stats stats;

    virtual public void OnDestroy()
    {
        if (stats)
        {
            stats.ManaUsed -= UseMana;
            stats.ManaRestored -= RestoreMana;
        }
    }

    protected void Update()
    {
        if (stats)
        {
            SetMaxMana(stats.GetMaxMana());
            mana = Mathf.Clamp(mana, 0, maxMana);
            UpdateManaUI();
        }
    }

    virtual public void UpdateManaUI()
    {
        float fillF = frontManaBar.fillAmount;
        float fillB = backManaBar.fillAmount;
        float mFraction = mana / maxMana;

        if (fillB > mFraction)
        {
            frontManaBar.fillAmount = mFraction;
            backManaBar.color = Color.white;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backManaBar.fillAmount = Mathf.Lerp(fillB, mFraction, percentComplete);
        }

        if (fillF < mFraction)
        {
            backManaBar.color = Color.blue; // Color change to represent mana
            backManaBar.fillAmount = mFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontManaBar.fillAmount = Mathf.Lerp(fillF, backManaBar.fillAmount, percentComplete);
        }
    }

    virtual public void UseMana(float amount)
    {
        mana -= amount;
        lerpTimer = 0f;
    }

    public void RestoreMana(float amount)
    {
        mana += amount;
        lerpTimer = 0f;
    }

    protected void SetMaxMana(float newMaxMana)
    {
        maxMana = newMaxMana;
    }
}
