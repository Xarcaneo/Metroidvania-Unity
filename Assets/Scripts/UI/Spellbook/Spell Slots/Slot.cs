using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

/// <summary>
/// Represents a UI slot that can hold and display spell data.
/// Handles selection, deselection, and spell assignment.
/// </summary>
public class Slot : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("Focus Frame")]
    [Tooltip("The UI element that highlights this slot when focused.")]
    [SerializeField] private RectTransform focusFrame;

    [Header("UI Elements")]
    [Tooltip("The UI Image to display the spell's icon.")]
    [SerializeField] private Image spellIconImage;

    [Tooltip("Optional reference to a lock icon Image component")]
    [SerializeField] private Image lockIcon;

    /// <summary>
    /// The data of the spell displayed in this slot.
    /// </summary>
    public SpellData AssignedSpell { get; private set; }

    private Button button;

    /// <summary>
    /// Event triggered when this slot is selected.
    /// </summary>
    public event Action<Slot> OnSlotSelected;

    /// <summary>
    /// Event triggered when this slot is deselected.
    /// </summary>
    public event Action<Slot> OnSlotDeselected;

    /// <summary>
    /// Whether this slot is currently locked.
    /// </summary>
    public bool IsLocked => isLocked;
    private bool isLocked = false;

    private void Awake()
    {
        // Cache the Button component
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError($"No Button component found on {gameObject.name}. This script requires a Button.");
        }
    }

    /// <summary>
    /// Called when this slot is selected (focused) in the UI.
    /// Moves and shows the focus frame at the position of this slot.
    /// </summary>
    /// <param name="eventData">Event data for the selection event.</param>
    public void OnSelect(BaseEventData eventData)
    {
        if (focusFrame != null)
        {
            focusFrame.position = transform.position;
            focusFrame.gameObject.SetActive(true);
        }

        // Trigger the OnSlotSelected event
        OnSlotSelected?.Invoke(this);
    }

    /// <summary>
    /// Called when this slot is deselected in the UI.
    /// Hides the focus frame.
    /// </summary>
    /// <param name="eventData">Event data for the deselection event.</param>
    public void OnDeselect(BaseEventData eventData)
    {
        if (focusFrame != null)
        {
            focusFrame.gameObject.SetActive(false);
        }

        // Trigger the OnSlotDeselected event
        OnSlotDeselected?.Invoke(this);
    }

    /// <summary>
    /// Assigns a spell to this slot and updates its UI.
    /// </summary>
    /// <param name="spell">The spell to assign.</param>
    public void AssignSpell(SpellData spellData)
    {
        spellIconImage.gameObject.SetActive(true);
        spellIconImage.sprite = spellData.SpellIcon;
        AssignedSpell = spellData;
        UpdateVisuals();
    }

    /// <summary>
    /// Sets whether this slot is locked and updates visuals accordingly.
    /// </summary>
    /// <param name="locked">Whether the slot should be locked.</param>
    public void SetLocked(bool locked)
    {
        isLocked = locked;

        if (lockIcon != null)
        {
            lockIcon.gameObject.SetActive(locked);
        }
    }

    /// <summary>
    /// Clears the slot's data and UI.
    /// </summary>
    public void ClearSlot()
    {
        spellIconImage.gameObject.SetActive(false);
        spellIconImage.sprite = null;
        AssignedSpell = default;
        isLocked = false;

        // Hide the lock icon for empty slots
        if (lockIcon != null)
        {
            lockIcon.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Updates the visual elements of the slot based on current state.
    /// </summary>
    private void UpdateVisuals()
    {
        bool hasSpell = !AssignedSpell.Equals(default);

        if (spellIconImage != null)
        {
            spellIconImage.enabled = hasSpell;
        }

        if (lockIcon != null)
        {
            lockIcon.gameObject.SetActive(hasSpell && isLocked);
        }
    }
}
