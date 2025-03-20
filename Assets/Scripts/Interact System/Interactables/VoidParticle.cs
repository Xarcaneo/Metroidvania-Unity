using UnityEngine;
using PixelCrushers.DialogueSystem;
using System.Collections;

/// <summary>
/// Represents a void particle that can be collected to unlock a spell.
/// When interacted with, it unlocks the associated spell and destroys itself.
/// </summary>
public class VoidParticle : Interactable
{
    [Header("Spell Unlock Settings")]
    [Tooltip("The ID of the spell to unlock (without prefix)")]
    [SerializeField] private string spellUnlockID;

    /// <summary>
    /// Initializes the void particle state.
    /// Waits one frame to ensure Lua system is ready.
    /// </summary>
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        // Check if this particle has already been collected
        string fullVariableName = SpellData.SPELL_PREFIX + spellUnlockID;
        bool isCollected = DialogueLua.GetVariable(fullVariableName).asBool;

        // If already collected, destroy this particle
        if (isCollected)
        {
            Destroy(gameObject);
        }
    }

    public override void Interact()
    {
        base.Interact();

        // Set the spell as unlocked in Lua
        string fullVariableName = SpellData.SPELL_PREFIX + spellUnlockID;
        DialogueLua.SetVariable(fullVariableName, true);

        // Notify any listeners that interaction is complete
        CallInteractionCompletedEvent();

        // Destroy the void particle
        Destroy(gameObject);
    }
}
