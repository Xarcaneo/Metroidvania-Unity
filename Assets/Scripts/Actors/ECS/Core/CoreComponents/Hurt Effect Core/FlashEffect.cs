using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FlashEffect : HurtEffect
{
    [Tooltip("Material to switch to during the flash.")]
    [SerializeField] private Material flashMaterial;

    [Tooltip("The material that was in use, when the script started.")]
    [SerializeField] private Material originalMaterial;

    public override IEnumerator EffectRoutine()
    {
        // Swap to the flashMaterial.
        spriteRenderer.material = flashMaterial;

        // Pause the execution of this function for "duration" seconds.
        yield return new WaitForSeconds(duration);

        // After the pause, swap back to the original material.
        spriteRenderer.material = originalMaterial;

        // Set the routine to null, signaling that it's finished.
        effectRoutine = null;
    }
}