using UnityEngine;

/// <summary>
/// Base class for all spell effects. Each effect defines a specific behavior.
/// </summary>
public abstract class SpellEffect : ScriptableObject
{
    [Header("Channeling Time")]
    [Tooltip("Time in seconds before the effect is triggered (e.g., for animations).")]
    public float channelingTime = 0f;

    /// <summary>
    /// Executes the effect on the target. Should be overridden by derived classes.
    /// </summary>
    /// <param name="caster">The GameObject casting the spell.</param>
    /// <param name="target">The target GameObject of the spell (can be null).</param>
    public abstract void TriggerEffect(GameObject caster, GameObject target);
}
