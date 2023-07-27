using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintBox : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1.0f; // Duration of the fade effect in seconds
    private CanvasGroup canvasGroup;
    private bool isFading;
    private Coroutine fadeCoroutine;
    private float remainingFadeTime;
    private float startAlpha;
    private float targetAlpha;
    private bool isPaused; // New variable to track pausing state

    private void OnEnable() => GameEvents.Instance.onEndSession += ResetAlpha;


    private void OnDisable()
    {
        GameEvents.Instance.onEndSession -= ResetAlpha;

        // Check if the hint box is in the middle of fading and adjust the alpha accordingly
        if (isFading)
        {
            canvasGroup.alpha = targetAlpha; // Set the alpha to the target value (max or 0)
            StopCoroutine(fadeCoroutine);
            isFading = false;
        }
    }

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Set the starting alpha to 0 to hide the object from the beginning
        ResetAlpha();
    }

    public void SetHintText(string text)
    {
        // Assuming you have a TextMeshProUGUI component for the hint text
        TextMeshProUGUI hintText = GetComponentInChildren<TextMeshProUGUI>();
        LocalizeUI localization = GetComponentInChildren<LocalizeUI>();

        if (hintText != null)
        {
            hintText.text = text;
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
    }

    public void FadeIn()
    {
        if (isFading)
        {
            // If already fading, stop the ongoing fade and start a new one from the current alpha
            StopCoroutine(fadeCoroutine);
        }

        startAlpha = canvasGroup.alpha;
        targetAlpha = 1f;
        remainingFadeTime = fadeDuration * (1f - startAlpha);
        fadeCoroutine = StartCoroutine(FadeCanvasGroupAlpha());
    }

    public void FadeOut()
    {
        if (isFading)
        {
            // If already fading, stop the ongoing fade and start a new one from the current alpha
            StopCoroutine(fadeCoroutine);
        }

        startAlpha = canvasGroup.alpha;
        targetAlpha = 0f;
        remainingFadeTime = fadeDuration * startAlpha;
        fadeCoroutine = StartCoroutine(FadeCanvasGroupAlpha());
    }

    private IEnumerator FadeCanvasGroupAlpha()
    {
        isFading = true;

        float elapsedTime = 0f;

        while (elapsedTime < remainingFadeTime)
        {
            if (isPaused)
            {
                // Pause the fading process by saving the remaining time
                yield return null;
            }
            else
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / remainingFadeTime);
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return null;
            }
        }

        canvasGroup.alpha = targetAlpha;
        isFading = false;
    }

    public void ResetAlpha()
    {
        // Set the alpha value to 0 to hide the object
        canvasGroup.alpha = 0f;
        isFading = false;
        isPaused = false; // Reset the pausing state when resetting the alpha
    }
}
