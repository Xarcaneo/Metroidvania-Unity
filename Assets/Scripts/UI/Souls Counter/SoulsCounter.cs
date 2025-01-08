using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the souls counter UI and handles soul collection animations.
/// Optimized for performance with string caching and smart animation scaling.
/// </summary>
public class SoulsCounter : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>
    /// Reference to the TextMeshProUGUI component that displays the souls count.
    /// </summary>
    [SerializeField] private TextMeshProUGUI counter;

    /// <summary>
    /// Base speed for counter animation updates in seconds.
    /// </summary>
    [SerializeField] private float baseCounterSpeed = 0.05f;

    /// <summary>
    /// Threshold at which the counter switches to faster increment mode.
    /// </summary>
    [SerializeField] private int largeNumberThreshold = 100;
    #endregion

    #region Private Fields
    /// <summary>
    /// Current number of souls displayed in the counter.
    /// </summary>
    private int currentSouls;

    /// <summary>
    /// Number of souls waiting to be added to the counter.
    /// </summary>
    private int pendingSouls;

    /// <summary>
    /// Flag indicating if the counter is currently updating.
    /// </summary>
    private bool isUpdating;

    /// <summary>
    /// Reference to the active update coroutine.
    /// </summary>
    private Coroutine updateCoroutine;

    /// <summary>
    /// StringBuilder instance for optimized string operations.
    /// </summary>
    private StringBuilder stringBuilder;

    /// <summary>
    /// Constant name of the souls variable in DialogueLua.
    /// </summary>
    private const string SOULS_VARIABLE = "Souls";

    /// <summary>
    /// Size of the number string cache for optimization.
    /// </summary>
    private const int CACHE_SIZE = 1000;

    /// <summary>
    /// Cache of pre-generated number strings for frequent updates.
    /// </summary>
    private string[] numberCache;
    #endregion

    #region Initialization
    /// <summary>
    /// Initializes the component and sets up caching systems.
    /// </summary>
    private void Awake()
    {
        stringBuilder = new StringBuilder(16);
        InitializeNumberCache();
    }

    /// <summary>
    /// Initializes the cache of commonly used number strings.
    /// </summary>
    private void InitializeNumberCache()
    {
        numberCache = new string[CACHE_SIZE];
        for (int i = 0; i < CACHE_SIZE; i++)
        {
            numberCache[i] = i.ToString();
        }
    }
    #endregion

    #region Event Subscriptions
    /// <summary>
    /// Subscribes to necessary game events when the component is enabled.
    /// </summary>
    private void OnEnable()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onSoulsReceived += OnSoulsReceived;
            GameEvents.Instance.onNewSession += OnNewSession;
        }
    }

    /// <summary>
    /// Unsubscribes from game events when the component is disabled.
    /// </summary>
    private void OnDisable()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onSoulsReceived -= OnSoulsReceived;
            GameEvents.Instance.onNewSession -= OnNewSession;
        }
        StopAllCoroutines();
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Handles the start of a new game session by updating the souls counter.
    /// </summary>
    private void OnNewSession()
    {
        StopUpdateCoroutineIfRunning();
        updateCoroutine = StartCoroutine(UpdateData());
    }

    /// <summary>
    /// Handles receiving new souls, updates the counter and triggers animation.
    /// </summary>
    /// <param name="soulsAmount">Amount of souls received</param>
    private void OnSoulsReceived(int soulsAmount)
    {
        pendingSouls += soulsAmount;
        DialogueLua.SetVariable(SOULS_VARIABLE, currentSouls + soulsAmount);

        if (!isUpdating)
        {
            StopUpdateCoroutineIfRunning();
            updateCoroutine = StartCoroutine(UpdateCounter());
        }
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// Updates the souls counter data from the dialogue system.
    /// Ensures proper initialization after frame rendering.
    /// </summary>
    private IEnumerator UpdateData()
    {
        yield return new WaitForEndOfFrame();

        pendingSouls = 0;
        currentSouls = DialogueLua.GetVariable(SOULS_VARIABLE).asInt;
        UpdateCounterText(currentSouls);
    }

    /// <summary>
    /// Handles the smooth animation of the souls counter with dynamic speed adjustment.
    /// Processes pending souls with optimized increment calculations.
    /// </summary>
    private IEnumerator UpdateCounter()
    {
        isUpdating = true;

        while (pendingSouls > 0)
        {
            int increment = CalculateIncrement(pendingSouls);
            float adjustedSpeed = CalculateSpeed(pendingSouls);

            currentSouls += increment;
            pendingSouls -= increment;

            UpdateCounterText(currentSouls);
            yield return new WaitForSeconds(adjustedSpeed);
        }

        isUpdating = false;
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Stops the current update coroutine if one is running.
    /// </summary>
    private void StopUpdateCoroutineIfRunning()
    {
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;
        }
    }

    /// <summary>
    /// Updates the counter text with optimized string handling.
    /// Uses cached strings for small numbers and StringBuilder for large ones.
    /// </summary>
    /// <param name="value">The value to display in the counter</param>
    private void UpdateCounterText(int value)
    {
        if (value < CACHE_SIZE)
        {
            counter.text = numberCache[value];
        }
        else
        {
            stringBuilder.Clear();
            stringBuilder.Append(value);
            counter.text = stringBuilder.ToString();
        }
    }

    /// <summary>
    /// Calculates the optimal increment size based on remaining souls.
    /// Provides faster increments for large numbers to maintain smooth animation.
    /// </summary>
    /// <param name="remaining">Number of souls remaining to be added</param>
    /// <returns>The calculated increment size</returns>
    private int CalculateIncrement(int remaining)
    {
        if (remaining > largeNumberThreshold)
        {
            return Mathf.Max(1, remaining / 20); // Faster increment for large numbers
        }
        return 1;
    }

    /// <summary>
    /// Calculates the optimal update speed based on remaining souls.
    /// Provides faster updates for large numbers to maintain reasonable animation duration.
    /// </summary>
    /// <param name="remaining">Number of souls remaining to be added</param>
    /// <returns>The calculated update speed in seconds</returns>
    private float CalculateSpeed(int remaining)
    {
        if (remaining > largeNumberThreshold)
        {
            return baseCounterSpeed * 0.5f; // Faster updates for large numbers
        }
        return baseCounterSpeed;
    }
    #endregion
}