using PixelCrushers;
using PixelCrushers.DialogueSystem;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the display of location names with fade animations when entering new areas.
/// </summary>
public class LocationNameIndicator : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI locationName;
    [SerializeField] private Image locationNameBackground;
    [SerializeField] private TextTable myTextTable;

    [Header("Animation Settings")]
    [SerializeField, Range(0.1f, 3f)] private float fadeInTime = 1f;
    [SerializeField, Range(0.5f, 5f)] private float timeBeforeFadeOut = 2f;
    [SerializeField, Range(0.1f, 3f)] private float fadeOutTime = 1f;

    private Coroutine fadeCoroutine;
    private bool isTransitioning;

    /// <summary>
    /// Initializes event subscriptions and ensures the indicator starts invisible.
    /// </summary>
    private void Awake()
    {
        GameEvents.Instance.onAreaChanged += OnAreaChanged;
        GameEvents.Instance.onPauseTrigger += OnPauseTrigger;
        GameEvents.Instance.onEndSession += OnEndSession;
        SetAlpha(0f); // Ensure we start invisible
    }

    /// <summary>
    /// Cleans up event subscriptions when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onAreaChanged -= OnAreaChanged;
            GameEvents.Instance.onPauseTrigger -= OnPauseTrigger;
            GameEvents.Instance.onEndSession -= OnEndSession;
        }
    }

    /// <summary>
    /// Handles the end of a game session by cleaning up animations and text.
    /// </summary>
    private void OnEndSession()
    {
        CancelFadeCoroutine();
        locationName.text = "";
    }

    /// <summary>
    /// Handles pause state changes by hiding the indicator when game is paused.
    /// </summary>
    /// <param name="isPaused">True if game is paused, false otherwise</param>
    private void OnPauseTrigger(bool isPaused)
    {
        if (isPaused)
        {
            CancelFadeCoroutine();
            SetAlpha(0f);
        }
    }

    /// <summary>
    /// Cancels any active fade animation and resets transition state.
    /// </summary>
    public void CancelFadeCoroutine()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        isTransitioning = false;
    }

    /// <summary>
    /// Legacy method for compatibility with LevelManager.
    /// Cancels current fade animation and resets text opacity.
    /// </summary>
    public void Cancelcoroutine()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            locationName.color = new Color(locationName.color.r, locationName.color.g, locationName.color.b, 0f);
            locationNameBackground.color = locationName.color;
        }
    }

    /// <summary>
    /// Handles area change events by updating and displaying the new area name.
    /// </summary>
    /// <param name="areaName">Name of the new area</param>
    private void OnAreaChanged(string areaName)
    {
        //if (string.IsNullOrEmpty(areaName)) return;

        //string localizedName = myTextTable.GetFieldTextForLanguage(areaName, Localization.Language);
        //if (string.IsNullOrEmpty(localizedName)) return;

        if (locationName.text != areaName)
        {
            locationName.text = areaName;
            StartFadeCoroutine();
        }
    }

    /// <summary>
    /// Starts the fade in/out animation sequence if not already transitioning.
    /// </summary>
    private void StartFadeCoroutine()
    {
        if (isTransitioning) return;
        CancelFadeCoroutine();
        fadeCoroutine = StartCoroutine(FadeInAndOut());
    }

    /// <summary>
    /// Coroutine that handles the fade in and out animation sequence.
    /// Fades in the text, waits, then fades it out.
    /// </summary>
    private IEnumerator FadeInAndOut()
    {
        isTransitioning = true;
        SetAlpha(0f);

        float currentTime = 0f;
        while (currentTime < fadeInTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeInTime);
            SetAlpha(alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }
        SetAlpha(1f);

        yield return new WaitForSecondsRealtime(timeBeforeFadeOut);

        currentTime = 0f;
        while (currentTime < fadeOutTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeOutTime);
            SetAlpha(alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }

        SetAlpha(0f);
        fadeCoroutine = null;
        isTransitioning = false;
    }

    /// <summary>
    /// Sets the alpha value for both the location name text and background.
    /// Ensures the alpha value is clamped between 0 and 1.
    /// </summary>
    /// <param name="alpha">Alpha value between 0 and 1</param>
    private void SetAlpha(float alpha)
    {
        alpha = Mathf.Clamp01(alpha);
        Color nameColor = locationName.color;
        Color bgColor = locationNameBackground.color;
        nameColor.a = alpha;
        bgColor.a = alpha;
        locationName.color = nameColor;
        locationNameBackground.color = bgColor;
    }
}
