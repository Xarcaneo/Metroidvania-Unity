using PixelCrushers;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;

public class HintBox : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] public TextMeshProUGUI hintText;
    [SerializeField] private LocalizeUI localization;

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
