using UnityEngine;
using TMPro;
using System.Collections;
using PixelCrushers;
using PixelCrushers.DialogueSystem;

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

    #region Private Fields
    /// <summary>
    /// Stores the initial hint text before any modifications.
    /// Used to preserve the original text with placeholders.
    /// </summary>
    private string initialHintText;
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
        string modifiedText = initialHintText;

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

    /// <summary>
    /// Initializes the hint box and subscribes to pause events.
    /// </summary>
    void Start()
    {
        GameEvents.Instance.onPauseTrigger += HandlePauseTrigger;
        StartCoroutine(WaitForText());
    }

    private IEnumerator WaitForText()
    {
        // Wait until either the TMP text or localization text is available
        while (string.IsNullOrEmpty(hintText.text))
        {
            yield return null;
        }
        
        initialHintText = hintText.text;
        SetHintText();
        HideHintBox(); // Hide the hint box after setting initial text
    }

    /// <summary>
    /// Handles pause/unpause events and updates hint text accordingly.
    /// </summary>
    /// <param name="isPaused">True if game is paused, false otherwise.</param>
    private void HandlePauseTrigger(bool isPaused)
    {
        if (!isPaused)
        {
            SetHintText();
        }
    }

    /// <summary>
    /// Cleans up event subscriptions when the object is destroyed.
    /// </summary>
    void OnDestroy()
    {
        GameEvents.Instance.onPauseTrigger -= HandlePauseTrigger;
    }
}
