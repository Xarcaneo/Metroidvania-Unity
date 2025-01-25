using UnityEngine;

/// <summary>
/// Represents a spell effect and its associated delay after execution.
/// </summary>
[System.Serializable]
public class EffectWithDelay
{
    /// <summary>
    /// The spell effect to apply.
    /// </summary>
    [Tooltip("The spell effect to apply.")]
    public SpellEffect effect;

    /// <summary>
    /// The delay (in seconds) after this effect is applied before the next effect occurs.
    /// </summary>
    [Tooltip("The delay (in seconds) after this effect is applied.")]
    public float delayAfterEffect = 0f;
}
