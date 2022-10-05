using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class HurtEffect : CoreComponent
{
    [Tooltip("The SpriteRenderer that should flash.")]
    protected SpriteRenderer spriteRenderer;

    [Tooltip("Duration of the flash.")]
    [SerializeField] protected float duration = 0.125f;

    // The currently running coroutine.
    protected Coroutine effectRoutine;

    protected Action<float> damageEventHandler;

    private void Start()
    {
        spriteRenderer = core.transform.parent.GetComponent<SpriteRenderer>();
        damageEventHandler = (amount) => { TurnOnEffect(); };
        core.Combat.OnDamage += damageEventHandler;
    }

    private void OnDisable()
    {
        core.Combat.OnDamage -= damageEventHandler;
    }
    public void TurnOnEffect()
    {
        // If the flashRoutine is not null, then it is currently running.
        if (effectRoutine != null)
        {
            // In this case, we should stop it first.
            // Multiple FlashRoutines the same time would cause bugs.
            StopCoroutine(effectRoutine);
        }

        // Start the Coroutine, and store the reference for it.
        effectRoutine = StartCoroutine(EffectRoutine());
    }

    public virtual IEnumerator EffectRoutine()
    {
        return null;
    }
}
