using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityCore.AudioManager;

/// <summary>
/// Represents a slot in the UI for a spell category.
/// Displays the category's name and icon, and handles user interaction.
/// </summary>
public class SpellCategorySlot : MonoBehaviour
{
    /// <summary>
    /// The TextMeshProUGUI component for displaying the category name.
    /// </summary>
    [Header("UI References")]
    [Tooltip("TextMeshPro field to display the category name.")]
    [SerializeField] private TextMeshProUGUI categoryNameText;

    /// <summary>
    /// The Image component for displaying the category icon.
    /// </summary>
    [Tooltip("Image component to display the category icon.")]
    [SerializeField] private Image categoryIconImage;

    /// <summary>
    /// The Button component for handling user interaction with this slot.
    /// </summary>
    [Tooltip("Button component for user interaction.")]
    [SerializeField] private Button slotButton;

    /// <summary>
    /// The frame image to display when this slot is focused.
    /// </summary>
    [Tooltip("Frame image to highlight this slot when focused.")]
    [SerializeField] private Image focusFrame;

    /// <summary>
    /// The SpellCategory assigned to this slot.
    /// </summary>
    [Header("Category Data")]
    [Tooltip("The SpellCategory assigned to this slot.")]
    [SerializeField] private SpellCategory spellCategory;

    /// <summary>
    /// Gets the SpellCategory assigned to this slot.
    /// </summary>
    public SpellCategory AssignedCategory => spellCategory;

    /// <summary>
    /// Assigns the category data to the UI elements.
    /// </summary>
    public void SetupCategorySlot()
    {
        if (spellCategory != null)
        {
            categoryNameText.text = spellCategory.categoryName;
            categoryIconImage.sprite = spellCategory.categoryIcon;
        }
        else
        {
            slotButton.interactable = false;
            gameObject.SetActive(false);
        }

        if (focusFrame != null)
        {
            focusFrame.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called when the slot gains focus or is selected.
    /// Activates the focus frame to visually indicate the slot is focused.
    /// </summary>
    public void OnSelect(bool isFirstSelection = false)
    {
        if (focusFrame != null)
        {
            focusFrame.gameObject.SetActive(true); // Show focus frame
        }

        // Play tab change sound when selecting category (except for first selection)
        if (!isFirstSelection)
        {
            AudioManager.instance.PlayUISound(AudioEventId.UI_Tab_Next);
        }
    }

    /// <summary>
    /// Called when the slot loses focus or is deselected.
    /// Deactivates the focus frame to visually indicate the slot is no longer focused.
    /// </summary>
    public void OnDeselect()
    {
        if (focusFrame != null)
        {
            focusFrame.gameObject.SetActive(false); // Hide focus frame
        }
    }
}
