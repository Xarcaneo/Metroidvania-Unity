using UnityEngine;

/// <summary>
/// Manages the navigation and focus of spell category slots.
/// Automatically selects the first slot and updates focus based on input events.
/// </summary>
public class SpellCategorySlotManager : MonoBehaviour
{
    [Header("Slot Manager Reference")]
    /// <summary>
    /// Reference to the SlotManager to retrieve and manage spell slots.
    /// </summary>
    [Tooltip("Reference to the SlotManager to manage spell slots dynamically.")]
    [SerializeField] private SlotManager slotManager;

    /// <summary>
    /// Array of all spell category slots managed by this manager.
    /// </summary>
    [Tooltip("Array of all spell category slots to manage.")]
    [SerializeField] private SpellCategorySlot[] slots;

    /// <summary>
    /// Array of spell slots to populate with spells from the selected category.
    /// </summary>
    [Tooltip("Spell slots to populate with spells from the selected category.")]
    [SerializeField] private Slot[] spellSlots;

    /// <summary>
    /// Index of the currently focused slot.
    /// </summary>
    private int currentSlotIndex = 0;

    /// <summary>
    /// Automatically called when the object is enabled.
    /// Selects the first slot.
    /// </summary>
    private void OnEnable()
    {
        // First refresh spell states from Lua
        if (SpellsCatalogue.Instance != null)
        {
            SpellsCatalogue.Instance.RefreshSpellStates();
        }

        // Then update UI
        UpdateAllSlots();
        SelectFirstSlot();
        SubscribeToInputEvents();
    }

    /// <summary>
    /// Automatically called when the object is disabled.
    /// Unsubscribes from input events.
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeFromInputEvents();
    }

    /// <summary>
    /// Subscribes to input events for navigating between slots.
    /// </summary>
    private void SubscribeToInputEvents()
    {
        if (InputManager.Instance == null)
            return;

        InputManager.Instance.OnCategoryUp += MoveFocusUp;
        InputManager.Instance.OnCategoryDown += MoveFocusDown;
    }

    /// <summary>
    /// Unsubscribes from input events to prevent memory leaks.
    /// </summary>
    private void UnsubscribeFromInputEvents()
    {
        if (InputManager.Instance == null)
            return;

        InputManager.Instance.OnCategoryUp -= MoveFocusUp;
        InputManager.Instance.OnCategoryDown -= MoveFocusDown;
    }

    /// <summary>
    /// Selects the first slot and updates its focus state.
    /// </summary>
    private void SelectFirstSlot()
    {
        if (slots.Length == 0)
        {
            Debug.LogWarning("No slots assigned to SpellCategorySlotManager.");
            return;
        }

        currentSlotIndex = 0;
        UpdateSlotFocus(currentSlotIndex);
    }

    /// <summary>
    /// Moves the focus to the previous slot.
    /// </summary>
    private void MoveFocusUp()
    {
        if (slots.Length == 0) return;

        currentSlotIndex = (currentSlotIndex - 1 + slots.Length) % slots.Length; // Wrap-around navigation
        UpdateSlotFocus(currentSlotIndex);
    }

    /// <summary>
    /// Moves the focus to the next slot.
    /// </summary>
    private void MoveFocusDown()
    {
        if (slots.Length == 0) return;

        currentSlotIndex = (currentSlotIndex + 1) % slots.Length; // Wrap-around navigation
        UpdateSlotFocus(currentSlotIndex);
    }

    /// <summary>
    /// Updates the focus state of all slots based on the current selection index.
    /// </summary>
    /// <param name="index">The index of the slot to focus.</param>
    private void UpdateSlotFocus(int index)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i == index)
            {
                slots[i].OnSelect(); // Highlight the currently focused slot
                PopulateSpellSlots(slots[i].AssignedCategory);
                slotManager.SelectFirstSlot();
            }
            else
            {
                slots[i].OnDeselect(); // Remove highlight from other slots
            }
        }
    }

    /// <summary>
    /// Updates the data for all slots by calling their SetupCategorySlot method.
    /// Ensures that each slot's UI elements are refreshed based on their assigned category data.
    /// </summary>
    private void UpdateAllSlots()
    {
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                slot.SetupCategorySlot(); // Refresh the slot's UI
            }
        }
    }

    /// <summary>
    /// Forces a refresh of the current category's spells.
    /// Call this after spell states have been updated.
    /// </summary>
    public void RefreshCurrentCategory()
    {
        if (slots == null || slots.Length == 0 || currentSlotIndex >= slots.Length) return;
        
        var currentCategory = slots[currentSlotIndex].AssignedCategory;
        if (currentCategory != null)
        {
            PopulateSpellSlots(currentCategory);
        }
    }

    /// <summary>
    /// Populates the spell slots with spells from the selected category.
    /// Shows all spells, with locked spells appearing disabled.
    /// </summary>
    /// <param name="category">The selected spell category.</param>
    private void PopulateSpellSlots(SpellCategory category)
    {
        ClearAllSpellSlots();

        if (category == null || category.spells == null) return;

        for (int i = 0; i < category.spells.Count && i < spellSlots.Length; i++)
        {
            var spell = category.spells[i];
            if (spell == null) continue;

            // Assign the spell and set its locked state
            spellSlots[i].AssignSpell(spell.spellData);
            spellSlots[i].SetLocked(!spell.IsUnlocked);
        }
    }

    /// <summary>
    /// Clears all spell slots.
    /// </summary>
    private void ClearAllSpellSlots()
    {
        foreach (var slot in spellSlots)
        {
            slot.ClearSlot();
        }
    }
}
