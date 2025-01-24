using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Represents a UI slot that can be focused and interacted with.
/// Includes functionality for highlighting a frame when the slot is focused.
/// </summary>
public class Slot : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    /// <summary>
    /// The frame that highlights this slot when it is focused.
    /// This should be a RectTransform representing a UI element, such as a border or glow effect.
    /// </summary>
    [Header("Focus Frame")]
    [Tooltip("The UI element that highlights this slot when focused.")]
    [SerializeField] private RectTransform focusFrame;

    /// <summary>
    /// Cached reference to the Button component attached to this slot.
    /// Used to ensure the slot is interactable.
    /// </summary>
    private Button button;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Caches the Button component and ensures the focus frame is hidden initially.
    /// </summary>
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
            // Align the focus frame to the position of this slot
            focusFrame.position = transform.position;
            focusFrame.gameObject.SetActive(true);
        }
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
    }
}
