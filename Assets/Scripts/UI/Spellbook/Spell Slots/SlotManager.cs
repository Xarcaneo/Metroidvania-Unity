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
    /// <summary>
    /// Reference to the SpellDescriptionUI component that displays the spell's details.
    /// Used to update the name and description of the selected spell.
    /// </summary>
    [Tooltip("Reference to the SpellDescriptionUI for displaying spell details.")]
    [SerializeField] private SpellDescriptionUI spellDescriptionUI;

    /// <summary>
    /// List of all slots under the grid container.
    /// </summary>
    private List<Slot> slots = new List<Slot>();

    /// <summary>
    /// Called when the object is enabled.
    /// Dynamically gathers all slots and focuses the first slot.
    /// </summary>
    private void OnEnable()
    {
        InitializeSlots();
        SelectFirstSlot();
    }

    /// <summary>
    /// Called when the object is disabled.
    /// Unsubscribes from all slot events to avoid memory leaks.
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeFromSlots();
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
        if (slots == null || slots.Count == 0)
        {
            return;
        }

        // Get the first slot from the list
        Slot firstSlot = slots[0];

        if (firstSlot == null)
        {
            Debug.LogWarning("The first slot in the list is null.");
            return;
        }

        // Set the first slot as the selected GameObject in the EventSystem
        EventSystem.current.SetSelectedGameObject(firstSlot.gameObject);

        // Trigger the slot's custom OnSelect logic (if applicable)
        firstSlot.OnSelect(null);
    }

    /// <summary>
    /// Handles slot selection to update the spell description UI.
    /// </summary>
    /// <param name="selectedSlot">The slot that was selected.</param>
    private void HandleSlotSelected(Slot selectedSlot)
    {
        if (selectedSlot != null)
        {
            spellDescriptionUI.UpdateSpellDescription(selectedSlot.AssignedSpell);
        }
    }

    /// <summary>
    /// Handles slot deselection to clear the spell description UI.
    /// </summary>
    /// <param name="deselectedSlot">The slot that was deselected.</param>
    private void HandleSlotDeselected(Slot deselectedSlot) => spellDescriptionUI.ClearDescription();
}
