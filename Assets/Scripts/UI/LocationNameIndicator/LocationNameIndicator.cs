using PixelCrushers;
using PixelCrushers.DialogueSystem;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationNameIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI locationName;
    [SerializeField] private Image locationNameBackground;
    [SerializeField] private TextTable myTextTable;

    private float fadeInTime = 1f;
    private float timeBeforeFadeOut = 2f;
    private float fadeOutTime = 1f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        GameEvents.Instance.onAreaChanged += OnAreaChanged;
        GameEvents.Instance.onPauseTrigger += OnPauseTrigger;
        GameEvents.Instance.onEndSession += OnEndSession;
    }

    private void OnDestroy()
    {
        GameEvents.Instance.onAreaChanged -= OnAreaChanged;
        GameEvents.Instance.onPauseTrigger -= OnPauseTrigger;
        GameEvents.Instance.onEndSession -= OnEndSession;
    }

    private void OnEndSession()
    {
        CancelFadeCoroutine();
        locationName.text = "";
    }

    private void OnPauseTrigger(bool isPaused)
    {
        if (isPaused)
        {
            CancelFadeCoroutine();
            SetAlpha(0f);
        }
    }

    public void CancelFadeCoroutine()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    public void Cancelcoroutine()
    {
        // Cancel the current fade coroutine if it is running
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            locationName.color = new Color(locationName.color.r, locationName.color.g, locationName.color.b, 0f);
            locationNameBackground.color = locationName.color;
        }
    }

    private void OnAreaChanged(string areaName)
    {
        areaName = myTextTable.GetFieldTextForLanguage(areaName, Localization.Language);
        if (locationName.text != areaName)
        {
            locationName.text = areaName;
            StartFadeCoroutine();
        }
    }

    private void StartFadeCoroutine()
    {
        CancelFadeCoroutine();
        fadeCoroutine = StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        SetAlpha(0f);

        float currentTime = 0f;
        while (currentTime < fadeInTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeInTime);
            SetAlpha(alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }

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
    }

    private void SetAlpha(float alpha)
    {
        Color nameColor = locationName.color;
        Color bgColor = locationNameBackground.color;
        nameColor.a = alpha;
        bgColor.a = alpha;
        locationName.color = nameColor;
        locationNameBackground.color = bgColor;
    }
}
