using System.Collections;
using TMPro;
using UnityEngine;

public class LocationNameIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI locationName;
    
    private float fadeInTime = 1f;
    private float timeBeforeFadeOut = 2f;
    private float fadeOutTime = 1f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        GameEvents.Instance.onAreaChanged += OnAreaChanged;
        GameEvents.Instance.onPauseTrigger += OnPauseTrigger;
    }

    private void OnDestroy()
    {
        GameEvents.Instance.onAreaChanged -= OnAreaChanged;
        GameEvents.Instance.onPauseTrigger -= OnPauseTrigger;
    }

    private void OnPauseTrigger(bool isPaused)
    {
        if (isPaused)
        {
            // Set the alpha to 0 when the game is paused
            locationName.color = new Color(locationName.color.r, locationName.color.g, locationName.color.b, 0f);

            // Cancel the current fade coroutine if it is running
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
        }
    }

    public void Cancelcoroutine()
    {
        // Cancel the current fade coroutine if it is running
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            locationName.color = new Color(locationName.color.r, locationName.color.g, locationName.color.b, 0f);
        }
    }

    private void OnAreaChanged(string areaName)
    {
        if (locationName.text != areaName)
        {
            locationName.text = areaName;
            fadeCoroutine = StartCoroutine(FadeInAndOut());
        }
    }
    private IEnumerator FadeInAndOut()
    {
        float currentTime = 0f;
        while (currentTime < fadeInTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeInTime);
            locationName.color = new Color(locationName.color.r, locationName.color.g, locationName.color.b, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(timeBeforeFadeOut);

        currentTime = 0f;
        while (currentTime < fadeOutTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeOutTime);
            locationName.color = new Color(locationName.color.r, locationName.color.g, locationName.color.b, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }
    }
}
