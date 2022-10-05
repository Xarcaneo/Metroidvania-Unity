using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorEffect : HurtEffect
{
    [Tooltip("Color to switch to during the effect.")]
    [SerializeField] private Color effectColor = new Color(255, 65, 65, 255);

    [Tooltip("The Color that was in use, when the script started.")]
    [SerializeField] private Color originalColor = new Color(255, 255, 255, 255);

    public override IEnumerator EffectRoutine()
    {
        // Swap to the new color.
        spriteRenderer.color = effectColor;

        // Pause the execution of this function for "duration" seconds.
        yield return new WaitForSeconds(duration);

        // After the pause, swap back to the original color.
        spriteRenderer.color = originalColor;

        // Set the routine to null, signaling that it's finished.
        effectRoutine = null;
    }
}
