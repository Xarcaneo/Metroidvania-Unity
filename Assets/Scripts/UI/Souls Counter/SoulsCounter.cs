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
    private int currentDisplayedSouls;

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
    /// Size of the number string cache for optimization.
    /// </summary>
    private const int CACHE_SIZE = 1000;

    /// <summary>
    /// Cache of pre-generated number strings for frequent updates.
    /// </summary>
    private string[] numberCache;

    /// <summary>
    /// Reference to the SoulsManager component.
    /// </summary>
    private SoulsManager soulsManager;

    /// <summary>
    /// Tracks if the counter is currently initialized.
    /// </summary>
    private bool isInitialized;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the component and sets up caching systems.
    /// </summary>
    private void Awake()
    {
        stringBuilder = new StringBuilder(16);
        InitializeNumberCache();
        Initialize();
    }

    private void OnEnable()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onPlayerSpawned += OnPlayerSpawned;
        }
    }

    private void OnDisable()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onPlayerSpawned -= OnPlayerSpawned;
        }


        StopAllCoroutines();
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Initializes the counter and subscribes to player events.
    /// </summary>
    private void Initialize()
    {
        if (isInitialized)
        {
            Debug.LogWarning("[SoulsCounter] Counter already initialized!");
            return;
        }

        if (GameEvents.Instance == null)
        {
            Debug.LogError("[SoulsCounter] GameEvents instance is null!");
            return;
        }

        isInitialized = true;
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

    #region Event Handlers
    /// <summary>
    /// Handles player spawn event by setting up souls manager reference.
    /// </summary>
    private void OnPlayerSpawned()
    {
        if (Player.Instance == null || Player.Instance.Core == null)
        {
            Debug.LogError("[SoulsCounter] Player instance or core is null!");
            return;
        }

        soulsManager = Player.Instance.Core.GetCoreComponent<SoulsManager>();
        if (soulsManager == null)
        {
            Debug.LogError("[SoulsCounter] Could not find SoulsManager component on player!");
            return;
        }

        // Start coroutine to wait for souls value
        StartCoroutine(WaitForSoulsValue());
    }

    /// <summary>
    /// Waits for SoulsManager to load the value before displaying it
    /// </summary>
    private IEnumerator WaitForSoulsValue()
    {
        yield return new WaitForEndOfFrame();
        
        // Get initial value
        currentDisplayedSouls = soulsManager.CurrentSouls;
        UpdateCounterText(currentDisplayedSouls);

        // Subscribe to future changes
        soulsManager.onSoulsValueChanged += OnSoulsValueChanged;
    }

    /// <summary>
    /// Handles souls value changes from SoulsManager
    /// </summary>
    private void OnSoulsValueChanged(int newValue)
    {
        int difference = newValue - currentDisplayedSouls;
        if (difference == 0) return;

        pendingSouls += difference;

        if (!gameObject.activeInHierarchy)
            return;

        if (!isUpdating)
        {
            StopUpdateCoroutineIfRunning();
            updateCoroutine = StartCoroutine(UpdateCounter());
        }
    }

    /// <summary>
    /// Handles receiving new souls during gameplay
    /// </summary>
    private void OnSoulsReceived(int amount)
    {
        if (amount <= 0) return;

        pendingSouls += amount;
        
        if (!isUpdating)
        {
            StopUpdateCoroutineIfRunning();
            updateCoroutine = StartCoroutine(UpdateCounter());
        }
    }
    #endregion

    #region UI Updates
    /// <summary>
    /// Updates the display to show current souls count.
    /// </summary>
    private void UpdateDisplay()
    {
        if (soulsManager != null)
        {
            currentDisplayedSouls = soulsManager.CurrentSouls;
            UpdateCounterText(currentDisplayedSouls);
        }
    }

    /// <summary>
    /// Coroutine that smoothly updates the counter display.
    /// </summary>
    private IEnumerator UpdateCounter()
    {
        isUpdating = true;

        while (pendingSouls != 0)
        {
            int step = pendingSouls > 0 ? 1 : -1;
            if (Mathf.Abs(pendingSouls) > largeNumberThreshold)
            {
                step *= 10;
            }

            currentDisplayedSouls += step;
            pendingSouls -= step;

            UpdateCounterText(currentDisplayedSouls);

            yield return new WaitForSeconds(GetUpdateDelay(Mathf.Abs(pendingSouls)));
        }

        isUpdating = false;
    }

    /// <summary>
    /// Gets the delay between counter updates based on remaining souls.
    /// </summary>
    private float GetUpdateDelay(int remaining)
    {
        return remaining > largeNumberThreshold ? baseCounterSpeed / 2f : baseCounterSpeed;
    }

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
    #endregion
}