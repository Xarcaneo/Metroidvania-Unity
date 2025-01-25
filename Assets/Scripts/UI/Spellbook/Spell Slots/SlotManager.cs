using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// Manages slot focus within a specific canvas.
/// Ensures the correct slot is focused when the canvas is activated.
/// </summary>
public class SlotManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Reference to the SpellDescriptionUI for displaying spell details.")]
    [SerializeField] private SpellDescriptionUI spellDescriptionUI;

    [Header("Hotbar Reference (Optional)")]
    [Tooltip("Reference to the SpellHotbar for assigning spells by ID to a specific hotbar slot.")]
    [SerializeField] private SpellHotbar spellHotbar;

    /// <summary>
    /// List of all slots under the grid container.
    /// </summary>
    private List<Slot> slots = new List<Slot>();

    /// <summary>
    /// Tracks the currently selected slot in the spellbook.
    /// </summary>
    private Slot currentlySelectedSlot;

    /// <summary>
    /// Called when the object is enabled.
    /// Dynamically gathers all slots, focuses the first slot, and subscribes to events.
    /// </summary>
    private void OnEnable()
    {
        InitializeSlots();
        SelectFirstSlot();

        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnAssignSpellToHotbar += HandleAssignSpellToHotbar;
        }
        else
        {
            Debug.LogWarning("InputManager instance is not available.");
        }
    }

    /// <summary>
    /// Called when the object is disabled.
    /// Unsubscribes from events and slot logic to avoid memory leaks.
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeFromSlots();

        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnAssignSpellToHotbar -= HandleAssignSpellToHotbar;
        }
    }

    /// <summary>
    /// Gathers all Slot components from the children of this GameObject
    /// and subscribes to their selection events.
    /// </summary>
    private void InitializeSlots()
    {
        // Clear the list to avoid duplication on re-enable
        slots.Clear();

        // Get all Slot components in the children of this GameObject
        slots.AddRange(GetComponentsInChildren<Slot>());

        // Subscribe to events for each slot
        foreach (var slot in slots)
        {
            slot.OnSlotSelected += HandleSlotSelected;
            slot.OnSlotDeselected += HandleSlotDeselected;
        }
    }

    /// <summary>
    /// Unsubscribes from all slot events.
    /// </summary>
    private void UnsubscribeFromSlots()
    {
        foreach (var slot in slots)
        {
            slot.OnSlotSelected -= HandleSlotSelected;
            slot.OnSlotDeselected -= HandleSlotDeselected;
        }
    }

    /// <summary>
    /// Public method to select the first slot in the list of slots.
    /// Ensures the first slot is focused and triggers custom logic if applicable.
    /// </summary>
    public void SelectFirstSlot()
    {
        // Check if there are any slots in the list
        if (slots == null || slots.Count == 0) return;

        // Get the first slot from the list
        Slot firstSlot = slots[0];
        if (firstSlot == null)
        {
            Debug.LogWarning("The first slot in the list is null.");
            return;
        }

        // Set the first slot as the selected GameObject in the EventSystem
        EventSystem.current.SetSelectedGameObject(firstSlot.gameObject);

        // Optionally call OnSelect manually:
        firstSlot.OnSelect(null);
    }

    /// <summary>
    /// Handles slot selection to update the spell description UI
    /// and track the currently selected slot.
    /// </summary>
    /// <param name="selectedSlot">The slot that was selected.</param>
    private void HandleSlotSelected(Slot selectedSlot)
    {
        if (selectedSlot != null)
        {
            currentlySelectedSlot = selectedSlot;
            spellDescriptionUI.UpdateSpellDescription(selectedSlot.AssignedSpell);
        }
    }

    /// <summary>
    /// Handles slot deselection to clear the spell description UI.
    /// Clears the reference if the deselected slot is the currently selected one.
    /// </summary>
    /// <param name="deselectedSlot">The slot that was deselected.</param>
    private void HandleSlotDeselected(Slot deselectedSlot)
    {
        if (currentlySelectedSlot == deselectedSlot)
        {
            currentlySelectedSlot = null;
        }
        spellDescriptionUI.ClearDescription();
    }

    /// <summary>
    /// Handles the AssignSpellToHotbar event (triggered by user input).
    /// Checks the currently selected slot for a spell, gets its ID,
    /// and optionally assigns it to a hotbar slot if a SpellHotbar is referenced.
    /// </summary>
    /// <param name="hotbarSlot">The hotbar slot number from user input.</param>
    private void HandleAssignSpellToHotbar(int hotbarSlot)
    {
        // Ensure there is a selected slot
        if (currentlySelectedSlot == null)
        {
            Debug.LogWarning("No slot is currently selected. Cannot assign a spell.");
            return;
        }

        // Ensure the selected slot has an assigned spell
        if (currentlySelectedSlot.AssignedSpell.Equals(default))
        {
            Debug.LogWarning("The currently selected slot does not contain a valid spell.");
            return;
        }

        int spellID = currentlySelectedSlot.AssignedSpell.SpellID;

        // If you have a SpellHotbar reference, pass the ID and the hotbar slot
        if (spellHotbar != null)
        {
            spellHotbar.AssignSpell(spellID, hotbarSlot);
        }
    }
}
