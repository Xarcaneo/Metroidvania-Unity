using UnityEngine;
using TMPro;
using System;
using System.Collections;

/// <summary>
/// Manages a UI hint box that displays contextual help text to the player.
/// Supports dynamic keybinding display without relying on localization.
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
    
    // We'll use the text directly from the TextMeshProUGUI component instead of a separate field
    #endregion

    #region Public Methods
    /// <summary>
    /// Updates the hint text with current keybindings.
    /// Replaces placeholders (e.g., {Attack}) with actual key names.
    /// </summary>
    /// <remarks>
    /// Format: Use {ActionName} in text to show current keybinding.
    /// Example: "Press {Attack} to attack" becomes "Press A to attack"
    /// </remarks>
    public void SetHintText()
    {
        if (hintText == null)
        {
            Debug.LogError($"[HintBox] TextMeshProUGUI component is missing on {gameObject.name}");
            return;
        }
        
        // Store the original text from the TextMeshProUGUI component
        string originalText = hintText.text;
        
        if (string.IsNullOrEmpty(originalText))
        {
            Debug.LogWarning($"[HintBox] No text set in TextMeshProUGUI component on {gameObject.name}");
            return;
        }

        string modifiedText = originalText;
        Debug.Log($"[HintBox] Processing text: '{originalText}' on {gameObject.name}");

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
            Debug.LogError($"[HintBox] Error setting text: {e.Message}\nText was: {originalText}");
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
        
        // Make sure text is set
        SetHintText();
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

        // Hide initially
        HideHintBox();

        // Subscribe to events if available
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onPauseTrigger += HandlePauseTrigger;
            GameEvents.Instance.onLanguageChanged += OnLanguageChanged;
        }
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
    /// Handles the language change event by updating the hint text.
    /// </summary>
    private void OnLanguageChanged()
    {
        // Re-process the text when language changes
        SetHintText();
    }

    /// <summary>
    /// Handles pause/unpause events and updates hint text accordingly.
    /// </summary>
    /// <param name="isPaused">True if game is paused, false otherwise.</param>
    private void HandlePauseTrigger(bool isPaused)
    {
        if (!isPaused)
        {
            // Re-process the text when unpausing
            SetHintText();
        }
    }
    #endregion
}
