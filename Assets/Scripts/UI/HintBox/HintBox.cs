using PixelCrushers;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages a UI hint box that displays contextual help text to the player.
/// Supports localization and dynamic keybinding display.
/// </summary>
public class HintBox : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>
    /// Canvas component for the hint box UI.
    /// Controls visibility of UI elements.
    /// </summary>
    [SerializeField] private Canvas canvas;

    /// <summary>
    /// Optional sprite renderer for non-UI visual elements.
    /// </summary>
    [SerializeField] private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Text component that displays the hint message.
    /// Supports rich text and keybinding placeholders.
    /// </summary>
    [SerializeField] public TextMeshProUGUI hintText;

    /// <summary>
    /// Handles translation of hint text to different languages.
    /// </summary>
    [SerializeField] private LocalizeUI localization;
    #endregion

    #region Public Methods
    /// <summary>
    /// Updates the hint text with current language and keybindings.
    /// Replaces placeholders (e.g., {Attack}) with actual key names.
    /// </summary>
    /// <remarks>
    /// Format: Use {ActionName} in text to show current keybinding.
    /// Example: "Press {Attack} to attack" becomes "Press A to attack"
    /// </remarks>
    public void SetHintText()
    {
        localization.enabled = true;
        string language = Localization.language;
        string modifiedText = hintText.text;

        // Find all placeholders in the text (e.g., {Attack}) and replace them with keybindings
        int startPlaceholderIndex = modifiedText.IndexOf('{');
        while (startPlaceholderIndex >= 0)
        {
            int endPlaceholderIndex = modifiedText.IndexOf('}', startPlaceholderIndex + 1);
            if (endPlaceholderIndex > startPlaceholderIndex)
            {
                string placeholder = modifiedText.Substring(startPlaceholderIndex + 1, endPlaceholderIndex - startPlaceholderIndex - 1);
                string keybinding = GameManager.Instance.GetKeybindingForAction(placeholder);
                modifiedText = modifiedText.Replace($"{{{placeholder}}}", keybinding);
            }

            startPlaceholderIndex = modifiedText.IndexOf('{', endPlaceholderIndex + 1);
        }

        hintText.SetText(modifiedText);
    }

    /// <summary>
    /// Makes the hint box visible by enabling its canvas and sprite.
    /// </summary>
    public void ShowHintBox()
    {
        if (canvas != null)
        {
            canvas.enabled = true;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
    }

    /// <summary>
    /// Hides the hint box by disabling its canvas and sprite.
    /// </summary>
    public void HideHintBox()
    {
        if (canvas != null)
        {
            canvas.enabled = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }
    #endregion
}
