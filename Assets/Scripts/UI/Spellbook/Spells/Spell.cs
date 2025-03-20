using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

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
    /// Whether this spell is currently unlocked
    /// </summary>
    private bool m_isUnlocked = false;

    /// <summary>
    /// Gets whether the spell is currently unlocked
    /// </summary>
    public bool IsUnlocked => m_isUnlocked;

    /// <summary>
    /// Initializes the spell's unlock state from the Dialogue System's Lua variables
    /// </summary>
    public void InitializeUnlockState()
    {
        if (string.IsNullOrEmpty(spellData.SpellUnlockID))
        {
            Debug.LogWarning($"[{spellData.SpellName}] SpellUnlockID is not set!");
            return;
        }

        string fullVariableName = SpellData.SPELL_PREFIX + spellData.SpellUnlockID;
        m_isUnlocked = DialogueLua.GetVariable(fullVariableName).asBool;
    }

    /// <summary>
    /// Casts the spell, applying its effects in sequence with delays.
    /// </summary>
    /// <param name="caster">The GameObject casting the spell.</param>
    /// <param name="target">The target GameObject (can be null).</param>
    public void Cast(Entity caster, Entity target)
    {
        if (!m_isUnlocked)
        {
            Debug.LogWarning($"Attempted to cast locked spell: {spellData.SpellName}");
            return;
        }

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
    private IEnumerator ApplyEffects(Entity caster, Entity target)
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
