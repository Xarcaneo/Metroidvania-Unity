using UnityEngine;

/// <summary>
/// Manages the navigation and focus of spell category slots.
/// Automatically selects the first slot when enabled and updates focus based on input events.
/// </summary>
public class SpellCategorySlotManager : MonoBehaviour
{
    /// <summary>
    /// Array of all spell category slots managed by this manager.
    /// </summary>
    [Tooltip("Array of all spell category slots to manage.")]
    [SerializeField] private SpellCategorySlot[] slots;

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
        InputManager.Instance.OnCategoryUp += MoveFocusUp;
        InputManager.Instance.OnCategoryDown += MoveFocusDown;
    }

    /// <summary>
    /// Unsubscribes from input events to prevent memory leaks.
    /// </summary>
    private void UnsubscribeFromInputEvents()
    {
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
}
