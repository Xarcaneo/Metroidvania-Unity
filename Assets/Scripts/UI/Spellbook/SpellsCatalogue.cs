using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A catalogue of all spells, providing global access and retrieval by ID.
/// </summary>
[CreateAssetMenu(fileName = "NewSpellsCatalogue", menuName = "SpellBook/SpellsCatalogue")]
public class SpellsCatalogue : ScriptableObject
{
    [Header("All Spells")]
    [Tooltip("List of all spells in the game.")]
    [SerializeField] private List<Spell> allSpells;

    /// <summary>
    /// The singleton instance of the SpellsCatalogue.
    /// </summary>
    public static SpellsCatalogue Instance { get; private set; }

    /// <summary>
    /// Called when the ScriptableObject is enabled.
    /// Ensures only one instance of SpellsCatalogue exists.
    /// </summary>
    private void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple instances of SpellsCatalogue detected. Only one instance should exist.");
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Retrieves a spell by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the spell to retrieve.</param>
    /// <returns>The spell with the matching ID, or null if not found.</returns>
    public Spell GetSpellByID(int id)
    {
        foreach (var spell in allSpells)
        {
            if (spell.spellData.SpellID == id)
            {
                return spell;
            }
        }

        Debug.LogWarning($"Spell with ID {id} not found in the catalogue.");
        return null;
    }
}
