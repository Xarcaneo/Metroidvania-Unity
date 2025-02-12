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
    /// Finds the slot ID that currently has the specified spell assigned.
    /// </summary>
    /// <param name="spellID">The ID of the spell to find</param>
    /// <returns>The slot ID where the spell is found, or -1 if not found</returns>
    private int FindSpellSlot(int spellID)
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            Spell slotSpell = hotbarSlots[i].GetAssignedSpell();
            if (slotSpell != null && slotSpell.spellData.SpellID == spellID)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Assigns a spell to the specified hotbar slot based on the provided spell ID.
    /// If the spell is already assigned to another slot, it will be moved to the new slot.
    /// If the new slot is occupied, the spells will be swapped.
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
        Spell newSpell = SpellsCatalogue.Instance.GetSpellByID(spellID);
        if (newSpell == null)
        {
            Debug.LogWarning($"No valid spell found for ID {spellID}.");
            return;
        }

        // Find if this spell is already assigned somewhere
        int existingSlot = FindSpellSlot(spellID);
        
        // Get the spell currently in the target slot
        Spell currentSpell = hotbarSlots[slotID].GetAssignedSpell();
        int currentSpellID = currentSpell?.spellData.SpellID ?? 0;

        if (existingSlot != -1)
        {
            // Spell is already assigned somewhere
            if (existingSlot == slotID)
            {
                // Spell is already in this slot, nothing to do
                return;
            }

            // Clear the spell from its old slot
            DialogueLua.SetVariable($"{StatePrefix}{existingSlot}", 0);
            hotbarSlots[existingSlot].AssignSpell(null);

            if (currentSpell != null)
            {
                // Move the current spell to the old slot
                DialogueLua.SetVariable($"{StatePrefix}{existingSlot}", currentSpellID);
                hotbarSlots[existingSlot].AssignSpell(currentSpell);
            }
        }
        else if (currentSpell != null)
        {
            // Target slot is occupied but spell isn't elsewhere, just keep the current spell where it is
            Debug.Log($"Assigning spell {newSpell.name} to slot {slotID}, replacing {currentSpell.name}");
        }

        // Assign the new spell to the target slot
        DialogueLua.SetVariable($"{StatePrefix}{slotID}", spellID);
        hotbarSlots[slotID].AssignSpell(newSpell);
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
            if (!luaResult.isNumber || luaResult.asInt == 0)
            {
                hotbarSlots[slotID].AssignSpell(null);
                continue;
            }

            int savedSpellID = luaResult.asInt;

            // Retrieve the spell from the catalogue
            Spell savedSpell = SpellsCatalogue.Instance.GetSpellByID(savedSpellID);

            // Check if the slot needs to be updated
            if (hotbarSlots[slotID].GetAssignedSpell() != savedSpell)
            {
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
    }

    /// <summary>
    /// Handles the onNewSession event by clearing the hotbar slots.
    /// </summary>
    private void OnNewSession()
    {
        ClearHotbar();
    }
}
