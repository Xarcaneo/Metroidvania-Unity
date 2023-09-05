using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintBox : MonoBehaviour
{
    private Canvas canvas;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas component not found in children.");
            return;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on this GameObject.");
            return;
        }
    }

        public void SetHintText(string text)
    {
        // Assuming you have a TextMeshProUGUI component for the hint text
        TextMeshProUGUI hintText = GetComponentInChildren<TextMeshProUGUI>();
        LocalizeUI localization = GetComponentInChildren<LocalizeUI>();

        localization.fieldName = text;
        localization.UpdateText();

        // Find all placeholders in the text (e.g., {Attack}) and replace them with keybindings
        string modifiedText = hintText.text;
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

        hintText.text = modifiedText;
    }
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
}
