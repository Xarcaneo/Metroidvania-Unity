using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a spell that can apply multiple effects with configurable delays between them.
/// </summary>
[CreateAssetMenu(fileName = "NewSpell", menuName = "SpellBook/Spell")]
public class Spell : ScriptableObject
{
    [Header("Spell Properties")]
    /// <summary>
    /// The core data of the spell, including name, icon, description, and ID.
    /// </summary>
    [Tooltip("The core data of the spell.")]
    public SpellData spellData;

    [Header("Spell Metadata")]
    [Tooltip("The type of the spell (e.g., Projectile, Utility, Buff).")]
    public SpellType spellType;

    [Tooltip("How the spell is cast (e.g., Instant, Channeled, Charged).")]
    public CastType castType;

    [Tooltip("Maximum channeling duration for channeled spells (in seconds).")]
    public float channelingTime = 0f;

    [Header("Spell Effects")]
    [Tooltip("The list of effects this spell applies, with configurable delays.")]
    public List<EffectWithDelay> effectsWithDelays;

    /// <summary>
    /// Casts the spell, applying its effects in sequence with delays.
    /// </summary>
    /// <param name="caster">The GameObject casting the spell.</param>
    /// <param name="target">The target GameObject (can be null).</param>
    public void Cast(GameObject caster, GameObject target)
    {
        if (effectsWithDelays == null || effectsWithDelays.Count == 0)
        {
            Debug.LogWarning($"{spellData.SpellName} has no effects to cast.");
            return;
        }

        // Start applying effects
        caster.GetComponent<MonoBehaviour>().StartCoroutine(ApplyEffects(caster, target));
    }

    /// <summary>
    /// Coroutine to apply each effect in sequence with its associated delay.
    /// </summary>
    /// <param name="caster">The GameObject casting the spell.</param>
    /// <param name="target">The target GameObject.</param>
    private IEnumerator ApplyEffects(GameObject caster, GameObject target)
    {
        foreach (var effectWithDelay in effectsWithDelays)
        {
            if (effectWithDelay.effect != null)
            {
                // Apply the effect
                effectWithDelay.effect.TriggerEffect(caster, target);
            }

            // Wait for the specified delay before the next effect
            yield return new WaitForSeconds(effectWithDelay.delayAfterEffect);
        }
    }
}
