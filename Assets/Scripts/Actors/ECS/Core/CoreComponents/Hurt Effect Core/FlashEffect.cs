using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Implements a flashing effect for when an entity is hurt.
/// Makes the sprite flash using a different material.
/// </summary>
public class FlashEffect : HurtEffect
{
    #region Settings

    /// <summary>
    /// Sound to play when hit.
    /// </summary>
    [SerializeField, Tooltip("Sound to play when hit")]
    private String sfx_patch;

    /// <summary>
    /// Material to switch to during the flash.
    /// </summary>
    [SerializeField, Tooltip("Material to switch to during the flash")]
    private Material flashMaterial;

    #endregion

    #region Private Fields

    /// <summary>
    /// Original material of the sprite.
    /// </summary>
    private Material originalMaterial;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Initializes the flash effect.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        originalMaterial = spriteRenderer.material;
    }

    #endregion

    #region Effect Implementation

    /// <summary>
    /// Coroutine that handles the flash effect animation.
    /// </summary>
    protected override IEnumerator EffectRoutine()
    {
        if (!string.IsNullOrEmpty(sfx_patch))
        {
            FMODUnity.RuntimeManager.PlayOneShot(sfx_patch, transform.position);
        }

        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(duration);
        spriteRenderer.material = originalMaterial;
    }

    #endregion
}