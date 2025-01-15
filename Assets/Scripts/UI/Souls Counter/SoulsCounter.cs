using TMPro;
using UnityEngine;

/// <summary>
/// Manages the souls counter UI and handles soul collection animations using LeanTween.
/// Simplified for easier maintenance by removing string caching and coroutine-based updates.
/// </summary>
public class SoulsCounter : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>
    /// Reference to the TextMeshProUGUI component that displays the souls count.
    /// </summary>
    [SerializeField] private TextMeshProUGUI counter;

    /// <summary>
    /// Base duration for counter animation updates in seconds.
    /// </summary>
    [SerializeField] private float baseCounterDuration = 0.5f;

    /// <summary>
    /// Threshold at which the counter switches to faster increment mode.
    /// </summary>
    [SerializeField] private int largeNumberThreshold = 100;
    #endregion

    #region Private Fields
    /// <summary>
    /// Reference to the SoulsManager component.
    /// </summary>
    private SoulsManager soulsManager;

    /// <summary>
    /// Reference to the active LeanTween animation.
    /// </summary>
    private LTDescr currentTween;

    /// <summary>
    /// Tracks if the counter is currently initialized.
    /// </summary>
    private bool isInitialized;

    /// <summary>
    /// Flag to determine if the initial soul count has been set.
    /// </summary>
    private bool isInitialSet = false;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the component and sets up necessary references.
    /// </summary>
    private void Awake()
    {
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

        // Kill any active tween to prevent memory leaks
        LeanTween.cancel(gameObject);

        // Unsubscribe from SoulsManager event to prevent potential null references
        if (soulsManager != null)
        {
            soulsManager.onSoulsValueChanged -= OnSoulsValueChanged;
        }
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
    #endregion

    #region Event Handlers
    /// <summary>
    /// Handles player spawn event by setting up SoulsManager reference.
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

        // Initialize the counter with the current souls value without animation
        UpdateCounterText(soulsManager.CurrentSouls);
        isInitialSet = true; // Set the flag to indicate initial value has been set

        // Subscribe to future changes
        soulsManager.onSoulsValueChanged += OnSoulsValueChanged;
    }

    /// <summary>
    /// Handles souls value changes from SoulsManager.
    /// </summary>
    /// <param name="newValue">The new souls count.</param>
    private void OnSoulsValueChanged(int newValue)
    {
        if (isInitialSet)
        {
            // Initial soul count has already been set; no animation needed
            UpdateCounterText(newValue);
            isInitialSet = false; // Reset the flag for future updates
            return;
        }

        int currentDisplayedSouls = GetCurrentDisplayedSouls();
        AnimateCounter(currentDisplayedSouls, newValue);
    }
    #endregion

    #region UI Updates
    /// <summary>
    /// Animates the counter from the current value to the new value using LeanTween.
    /// </summary>
    /// <param name="from">Starting value.</param>
    /// <param name="to">Ending value.</param>
    private void AnimateCounter(int from, int to)
    {
        // Cancel any existing tween
        if (currentTween != null)
        {
            LeanTween.cancel(currentTween.id);
        }

        // Calculate duration based on the difference
        int difference = Mathf.Abs(to - from);
        float duration = baseCounterDuration;

        if (difference > largeNumberThreshold)
        {
            duration *= 0.5f; // Faster animation for large changes
        }

        // Use LeanTween's value tweening
        currentTween = LeanTween.value(gameObject, from, to, duration)
            .setOnUpdate((float val) =>
            {
                UpdateCounterText(Mathf.RoundToInt(val));
            })
            .setEase(LeanTweenType.easeOutCubic)
            .setOnComplete(OnAnimationComplete);
    }

    /// <summary>
    /// Callback for when the animation completes.
    /// Adds a subtle scale animation for visual feedback.
    /// </summary>
    private void OnAnimationComplete()
    {
        // Add a subtle scale animation on completion for feedback
        LeanTween.scale(counter.gameObject, Vector3.one * 1.1f, 0.1f)
            .setLoopPingPong(1)
            .setEase(LeanTweenType.easeInOutSine);
    }

    /// <summary>
    /// Updates the counter text directly without using StringBuilder or caching.
    /// </summary>
    /// <param name="value">The value to display in the counter.</param>
    private void UpdateCounterText(int value)
    {
        counter.text = value.ToString();
    }

    /// <summary>
    /// Retrieves the current displayed souls from the counter text.
    /// </summary>
    /// <returns>The current souls count as an integer.</returns>
    private int GetCurrentDisplayedSouls()
    {
        if (int.TryParse(counter.text, out int displayedSouls))
        {
            return displayedSouls;
        }
        return 0;
    }
    #endregion
}
