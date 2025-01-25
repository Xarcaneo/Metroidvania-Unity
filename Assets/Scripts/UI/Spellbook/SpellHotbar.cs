using PixelCrushers.DialogueSystem;
using UnityEngine;
using System.Collections;

/// <summary>
/// Represents a basic SpellHotbar system that can assign multiple spells
/// from the SpellsCatalogue to different hotbar slots.
/// </summary>
public class SpellHotbar : MonoBehaviour
{
    /// <summary>
    /// Prefix for state variables in Lua
    /// </summary>
    protected const string StatePrefix = "SpellHotbar.";

    [Header("SpellHotbar Settings")]
    [Tooltip("Array of hotbar slots, indexed by slot ID. " +
             "Adjust the array size for the number of hotbar slots you need.")]
    [SerializeField] private SpellHotbarSlot[] hotbarSlots;

    private void Awake()
    {
        // Subscribe to the onNewSession event
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onNewSession += OnNewSession;
        }

        // Start a coroutine to delay the Lua synchronization
        StartCoroutine(DelayedSync());
    }

    private void OnDestroy()
    {
        // Unsubscribe from the onNewSession event
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onNewSession -= OnNewSession;
        }
    }

    private void OnEnable()
    {
        // Start a coroutine to delay the Lua synchronization
        StartCoroutine(DelayedSync());
    }

    /// <summary>
    /// Coroutine to wait one frame before synchronizing the hotbar with Lua variables.
    /// </summary>
    private IEnumerator DelayedSync()
    {
        yield return null; // Wait one frame
        SyncHotbarWithLua();
    }

    /// <summary>
    /// Assigns a spell to the specified hotbar slot based on the provided spell ID.
    /// </summary>
    /// <param name="spellID">The ID of the spell to assign.</param>
    /// <param name="slotID">The slot ID (index) in the hotbar to assign the spell to.</param>
    public void AssignSpell(int spellID, int slotID)
    {
        // Safety check: slotID must be within the array bounds
        if (slotID < 0 || slotID >= hotbarSlots.Length)
        {
            Debug.LogError($"SpellHotbar::AssignSpell - slotID {slotID} is out of range!");
            return;
        }

        // Retrieve the Spell from the SpellsCatalogue by ID
        Spell spell = SpellsCatalogue.Instance.GetSpellByID(spellID);
        if (spell == null)
        {
            Debug.LogWarning($"No valid spell found for ID {spellID}.");
            return;
        }

        // Assign the spell to the corresponding hotbar slot
        DialogueLua.SetVariable($"{StatePrefix}{slotID}", spellID); // Save to Lua
        hotbarSlots[slotID].AssignSpell(spell);
    }

    /// <summary>
    /// Synchronizes the hotbar slots with the state stored in Lua variables.
    /// </summary>
    private void SyncHotbarWithLua()
    {
        for (int slotID = 0; slotID < hotbarSlots.Length; slotID++)
        {
            // Retrieve the saved spell ID from Lua
            Lua.Result luaResult = DialogueLua.GetVariable($"{StatePrefix}{slotID}");
            if (!luaResult.isNumber)
            {
                Debug.LogWarning($"No valid spell ID stored for slot {slotID} in Lua. Clearing slot.");
                hotbarSlots[slotID].AssignSpell(null);
                continue;
            }

            int savedSpellID = luaResult.asInt;

            // Retrieve the spell from the catalogue
            Spell savedSpell = SpellsCatalogue.Instance.GetSpellByID(savedSpellID);

            // Check if the slot needs to be updated
            if (hotbarSlots[slotID].GetAssignedSpell() != savedSpell)
            {
                Debug.Log($"Synchronizing slot {slotID} with saved spell ID: {savedSpellID}.");
                hotbarSlots[slotID].AssignSpell(savedSpell);
            }
        }
    }

    /// <summary>
    /// Clears all hotbar slots by removing their assigned spells.
    /// </summary>
    public void ClearHotbar()
    {
        foreach (var slot in hotbarSlots)
        {
            if (slot != null)
            {
                slot.AssignSpell(null); // Clear the slot by assigning null
            }
        }

        Debug.Log("Hotbar slots cleared.");
    }

    /// <summary>
    /// Handles the onNewSession event by clearing the hotbar slots.
    /// </summary>
    private void OnNewSession()
    {
        ClearHotbar();
    }
}
