using UnityEngine;
using System.Collections;

/// <summary>
/// Implements a color change effect for when an entity is hurt.
/// Smoothly transitions the sprite between its normal color and a hurt color.
/// </summary>
public class ColorEffect : HurtEffect
{
    #region Settings

    /// <summary>
    /// Color to transition to when hurt.
    /// </summary>
    [SerializeField, Tooltip("Color to transition to when hurt")]
    private Color hurtColor = Color.red;

    /// <summary>
    /// Speed of the color transition (multiplier).
    /// Higher values make the transition faster.
    /// </summary>
    [SerializeField, Tooltip("Speed multiplier for color transition")]
    private float transitionSpeed = 5f;

    #endregion

    #region Private Fields

    /// <summary>
    /// Original color of the sprite.
    /// </summary>
    private Color originalColor;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Initializes the color effect.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        originalColor = spriteRenderer.color;
    }

    #endregion

    #region Effect Implementation

    /// <summary>
    /// Coroutine that handles the color transition effect.
    /// </summary>
    protected override IEnumerator EffectRoutine()
    {
        float currentTime = 0f;
        float transitionDuration = duration / transitionSpeed;
        float halfTransition = transitionDuration * 0.5f;

        // Transition to hurt color
        while (currentTime < halfTransition)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / halfTransition;
            spriteRenderer.color = Color.Lerp(originalColor, hurtColor, t);
            yield return null;
        }

        // Hold at hurt color briefly
        spriteRenderer.color = hurtColor;
        yield return new WaitForSeconds(0.1f);

        // Transition back to original color
        currentTime = 0f;
        while (currentTime < halfTransition)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / halfTransition;
            spriteRenderer.color = Color.Lerp(hurtColor, originalColor, t);
            yield return null;
        }

        // Ensure we end up with exactly the original color
        spriteRenderer.color = originalColor;
    }

    #endregion
}
