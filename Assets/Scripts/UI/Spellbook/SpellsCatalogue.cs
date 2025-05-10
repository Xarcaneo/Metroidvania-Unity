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

    private static SpellsCatalogue instance;
    
    /// <summary>
    /// The singleton instance of the SpellsCatalogue.
    /// </summary>
    public static SpellsCatalogue Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("SpellsCatalogue instance is null! Make sure to assign it in the GameManager or another initialization script.");
            }
            return instance;
        }
    }

    /// <summary>
    /// Initialize the SpellsCatalogue instance.
    /// Call this from a manager script in Awake or Start.
    /// </summary>
    public static void Initialize(SpellsCatalogue catalogue)
    {
        if (catalogue == null)
        {
            Debug.LogError("Trying to initialize SpellsCatalogue with null reference!");
            return;
        }
        
        instance = catalogue;
    }

    /// <summary>
    /// Called when the ScriptableObject is enabled.
    /// </summary>
    private void OnEnable()
    {
        // No need to set instance here anymore
    }

    private void OnDisable()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    /// <summary>
    /// Refreshes the unlock states for all spells in the catalogue.
    /// Call this when opening the spellbook to ensure states are up to date.
    /// </summary>
    public void RefreshSpellStates()
    {
        if (allSpells == null)
        {
            Debug.LogError("SpellsCatalogue: allSpells list is null!");
            return;
        }

        foreach (var spell in allSpells)
        {
            if (spell == null)
            {
                Debug.LogWarning("Null spell found in SpellsCatalogue!");
                continue;
            }

            spell.InitializeUnlockState();
        }
    }

    /// <summary>
    /// Retrieves a spell by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the spell to retrieve.</param>
    /// <returns>The spell with the matching ID, or null if not found.</returns>
    public Spell GetSpellByID(int id)
    {
        if (id == 0)
            return null;

        if (allSpells == null)
        {
            Debug.LogError("SpellsCatalogue: allSpells list is null!");
            return null;
        }

        foreach (var spell in allSpells)
        {
            if (spell == null)
            {
                Debug.LogWarning("Null spell found in SpellsCatalogue!");
                continue;
            }

            if (spell.spellData.SpellID == id)
            {
                return spell;
            }
        }

        Debug.LogWarning($"Spell with ID {id} not found in the catalogue.");
        return null;
    }
}
