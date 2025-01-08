using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles visual feedback for button focus states in the UI.
/// Changes text color based on button selection state.
/// </summary>
public class ButtonFocusDetector : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    #region Serialized Fields
    /// <summary>
    /// Color to use when the button is selected/focused.
    /// </summary>
    [SerializeField] private Color selectedColor;

    /// <summary>
    /// Color to use when the button is deselected/unfocused.
    /// </summary>
    [SerializeField] private Color deselectedColor;

    /// <summary>
    /// Text component to change color based on focus state.
    /// </summary>
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    /// <summary>
    /// If true, this button starts selected when enabled.
    /// </summary>
    [SerializeField] private bool isFirstButton;
    #endregion

    #region Unity Event Functions
    /// <summary>
    /// Sets initial text color when the button is enabled.
    /// Uses selected color if this is the first button, otherwise deselected color.
    /// </summary>
    private void OnEnable() => 
        textMeshProUGUI.color = isFirstButton ? selectedColor : deselectedColor;

    /// <summary>
    /// Resets text color to deselected state when button is disabled.
    /// </summary>
    private void OnDisable() => 
        textMeshProUGUI.color = deselectedColor;
    #endregion

    #region UI Event Handlers
    /// <summary>
    /// Changes text color to selected state when button gains focus.
    /// </summary>
    /// <param name="eventData">Event data from the UI system</param>
    public void OnSelect(BaseEventData eventData) => 
        textMeshProUGUI.color = selectedColor;

    /// <summary>
    /// Changes text color to deselected state when button loses focus.
    /// </summary>
    /// <param name="eventData">Event data from the UI system</param>
    public void OnDeselect(BaseEventData eventData) => 
        textMeshProUGUI.color = deselectedColor;
    #endregion
}
