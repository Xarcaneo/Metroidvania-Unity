using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : CoreComponent, IDamageable, IKnockbackable
{
    [Header("Knockback variables")]
    [SerializeField] private Vector2 knockbackAngle = new Vector2(1,0);
    [SerializeField] private float knockbackStrength = 2;
    [SerializeField] private float maxKnockbackTime = 0.2f;

    private bool isKnockbackActive;
    private float knockbackStartTime;

    public void LogicUpdate()
    {
        CheckKnockback();
    }

    public void Damage(float amount)
    {
        Debug.Log(core.transform.parent.name + " Damaged!");
    }

    public void Knockback(int direction)
    {
        Debug.Log(core.transform.parent.name + " Knockback!");
        core.Movement.SetVelocity(knockbackStrength, knockbackAngle, direction);
        core.Movement.CanSetVelocity = false;
        isKnockbackActive = true;
        knockbackStartTime = Time.time;
    }

    private void CheckKnockback()
    {
        if (isKnockbackActive && Time.time >= knockbackStartTime + maxKnockbackTime)
        {
            isKnockbackActive = false;
            core.Movement.CanSetVelocity = true;
        }
    }
}