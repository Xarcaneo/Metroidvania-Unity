using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    [Tooltip("Sound to play when hit.")]
    [SerializeField] private String sfx_patch;

    [Tooltip("Material to switch to during the flash.")]
    [SerializeField] private Material flashMaterial;

    [Tooltip("The material that was in use, when the script started.")]
    [SerializeField] private Material originalMaterial;

    [Tooltip("Duration of the flash.")]
    [SerializeField] private float duration = 0.125f;

    // Reference to the SpriteRenderer component.
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // Get the SpriteRenderer component attached to the GameObject.
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // OnTriggerEnter2D is called when a collider enters the trigger.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (sfx_patch != "")
            FMODUnity.RuntimeManager.PlayOneShot(sfx_patch, this.transform.position);

        // Start the flash effect coroutine.
        StartCoroutine(FlashEffectCoroutine());     
    }

    private IEnumerator FlashEffectCoroutine()
    {
        // Swap to the flashMaterial.
        spriteRenderer.material = flashMaterial;

        // Pause the execution of this function for "duration" seconds.
        yield return new WaitForSeconds(duration);

        // After the pause, swap back to the original material.
        spriteRenderer.material = originalMaterial;
    }
}
