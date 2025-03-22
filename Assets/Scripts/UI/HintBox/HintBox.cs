using UnityEngine;
using TMPro;
using System.Collections;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using System;

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
        if (string.IsNullOrEmpty(initialHintText))
        {
            Debug.LogWarning("[HintBox] Trying to set text but initialHintText is empty on " + gameObject.name);
            return;
        }

        if (hintText == null)
        {
            Debug.LogError("[HintBox] Trying to set text but hintText component is null on " + gameObject.name);
            return;
        }

        if (localization != null)
        {
            localization.enabled = true;
        }

        string modifiedText = initialHintText;

        try
        {
            int startIndex = 0;
            while (true)
            {
                // Find next placeholder
                int startPlaceholderIndex = modifiedText.IndexOf('{', startIndex);
                if (startPlaceholderIndex == -1) break; // No more placeholders

                int endPlaceholderIndex = modifiedText.IndexOf('}', startPlaceholderIndex);
                if (endPlaceholderIndex == -1) // No closing brace
                {
                    Debug.LogWarning($"[HintBox] Missing closing brace in text: {modifiedText}");
                    break;
                }

                // Extract placeholder and get keybinding
                string placeholder = modifiedText.Substring(startPlaceholderIndex + 1, 
                    endPlaceholderIndex - startPlaceholderIndex - 1);
                string keybinding = GameManager.Instance != null ? 
                    GameManager.Instance.GetKeybindingForAction(placeholder) : 
                    placeholder;

                // Replace this instance of the placeholder
                modifiedText = modifiedText.Remove(startPlaceholderIndex, 
                    endPlaceholderIndex - startPlaceholderIndex + 1)
                    .Insert(startPlaceholderIndex, keybinding);

                // Move start index past this replacement
                startIndex = startPlaceholderIndex + keybinding.Length;
            }

            hintText.SetText(modifiedText);
        }
        catch (Exception e)
        {
            Debug.LogError($"[HintBox] Error setting text: {e.Message}\nText was: {initialHintText}");
        }
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

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the hint box and subscribes to events.
    /// </summary>
    private void Start()
    {
        // Validate components
        if (hintText == null)
        {
            Debug.LogError("[HintBox] HintText component is missing");
            return;
        }

        // Hide initially until text is ready
        HideHintBox();

        // Subscribe to events if available
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onPauseTrigger += HandlePauseTrigger;
            GameEvents.Instance.onLanguageChanged += OnLanguageChanged;
        }

        // Start text initialization
        if (localization != null)
        {
            localization.enabled = true;
        }
        
        StartCoroutine(WaitForText());
    }

    /// <summary>
    /// Cleans up event subscriptions when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onPauseTrigger -= HandlePauseTrigger;
            GameEvents.Instance.onLanguageChanged -= OnLanguageChanged;
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Handles the language change event by updating the initial text.
    /// </summary>
    private void OnLanguageChanged() => initialHintText = hintText.text;

    /// <summary>
    /// Waits for the text component to be initialized with content.
    /// </summary>
    private IEnumerator WaitForText()
    {
        // Wait until either the TMP text is available and not empty
        while (hintText == null || string.IsNullOrEmpty(hintText.text))
        {
            yield return null;
        }
        
        // Store initial text only if we have valid text
        if (!string.IsNullOrEmpty(hintText.text))
        {
            initialHintText = hintText.text;
            SetHintText();
        }
    }

    /// <summary>
    /// Handles pause/unpause events and updates hint text accordingly.
    /// </summary>
    /// <param name="isPaused">True if game is paused, false otherwise.</param>
    private void HandlePauseTrigger(bool isPaused)
    {
        if (!isPaused && !string.IsNullOrEmpty(initialHintText))
        {
            SetHintText();
        }
    }
    #endregion
}
