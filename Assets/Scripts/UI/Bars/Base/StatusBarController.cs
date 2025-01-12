using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Base class for UI status bars that provides smooth value transitions and visual feedback.
/// Handles health, mana, and other status indicators with a dual-bar system for fluid animations.
/// Derived classes must implement stat-specific behavior through abstract methods.
/// </summary>
public abstract class StatusBarController : MonoBehaviour
{
    #region Serialized Fields
    [Header("UI Components")]
    /// <summary>
    /// Primary UI bar that shows the current value. Updates instantly when value decreases.
    /// </summary>
    [Tooltip("The front fill image of the status bar")]
    [SerializeField] protected Image frontBar;
    /// <summary>
    /// Secondary UI bar that creates transition effects. Updates instantly when value increases.
    /// </summary>
    [Tooltip("The back fill image of the status bar that shows transitions")]
    [SerializeField] protected Image backBar;

    [Header("Settings")]
    /// <summary>
    /// Speed of the transition animation between value changes. Higher values = slower transitions.
    /// </summary>
    [Tooltip("Speed at which the bar transitions when value changes")]
    [SerializeField] protected float chipSpeed = 2f;
    #endregion

    #region Protected Fields
    /// <summary>
    /// Color used when the value is being restored (e.g., healing).
    /// Set by derived classes in Awake().
    /// </summary>
    protected Color restoreColor = Color.white;
    /// <summary>
    /// Color used when the value is being depleted (e.g., damage).
    /// Set by derived classes in Awake().
    /// </summary>
    protected Color depleteColor = Color.white;
    /// <summary>
    /// Reference to the Stats component that provides the actual values.
    /// Found automatically in parent hierarchy.
    /// </summary>
    protected Stats stats;
    /// <summary>
    /// Current value of the status bar (e.g., current health/mana).
    /// Automatically clamped between 0 and maxValue.
    /// </summary>
    protected float currentValue;
    /// <summary>
    /// Maximum possible value for the status bar.
    /// Retrieved from Stats component through GetMaxValueFromStats().
    /// </summary>
    protected float maxValue = 100f;
    /// <summary>
    /// Timer used for smooth lerping transitions.
    /// Reset when value changes occur.
    /// </summary>
    private float lerpTimer;
    #endregion

    #region Unity Methods
    /// <summary>
    /// Validates required components on Awake.
    /// Ensures all necessary UI components are properly assigned and configured.
    /// </summary>
    protected virtual void Awake()
    {
        ValidateComponents();
    }

    /// <summary>
    /// Initializes the status bar on Start.
    /// Sets up the Stats component connection and subscribes to events.
    /// </summary>
    protected virtual void Start()
    {
        InitializeStats();
    }

    /// <summary>
    /// Updates the status bar every frame.
    /// - Updates maximum value from stats
    /// - Clamps current value within valid range
    /// - Updates UI with smooth transitions
    /// </summary>
    protected virtual void Update()
    {
        if (!stats) return;

        UpdateMaxValue(GetMaxValueFromStats());
        currentValue = GetCurrentValueFromStats(); // Get current value from Stats
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        UpdateUI();
    }

    /// <summary>
    /// Cleans up by unsubscribing from events when the component is destroyed.
    /// </summary>
    public virtual void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Updates the status bar UI with smooth transitions.
    /// 
    /// Behavior:
    /// - When value decreases:
    ///   * Front bar updates immediately
    ///   * Back bar smoothly transitions to new value
    ///   * Uses depleteColor for visual feedback
    /// 
    /// - When value increases:
    ///   * Back bar updates immediately
    ///   * Front bar smoothly transitions to new value
    ///   * Uses restoreColor for visual feedback
    /// 
    /// - Transition speed controlled by chipSpeed
    /// - Automatically handles rapid value changes
    /// </summary>
    public virtual void UpdateUI()
    {
        if (!ValidateUIComponents()) return;

        float fillF = frontBar.fillAmount;
        float fillB = backBar.fillAmount;
        float fraction = currentValue / maxValue;

        if (fillB > fraction)
        {
            UpdateDepleting(fraction, fillB, fillF);
        }

        if (fillF < fraction)
        {
            UpdateRestoring(fraction, fillF);
        }
    }

    /// <summary>
    /// Reduces the current value by the specified amount.
    /// 
    /// Features:
    /// - Automatically clamps to minimum of 0
    /// - Resets transition timer for smooth animation
    /// - Triggers UI update
    /// </summary>
    /// <param name="amount">Amount to reduce the value by</param>
    public virtual void ReduceValue(float amount)
    {
        currentValue = Mathf.Max(0, currentValue - amount);
        ResetLerpTimer();
    }

    /// <summary>
    /// Restores the value by the specified amount.
    /// 
    /// Features:
    /// - Automatically clamps to maximum value
    /// - Resets transition timer for smooth animation
    /// - Triggers UI update
    /// </summary>
    /// <param name="amount">Amount to restore the value by</param>
    public virtual void RestoreValue(float amount)
    {
        currentValue = Mathf.Min(maxValue, currentValue + amount);
        ResetLerpTimer();
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Get the maximum value from the Stats component.
    /// Must be implemented by derived classes to provide the correct maximum value.
    /// </summary>
    /// <returns>The maximum value for this status bar</returns>
    protected abstract float GetMaxValueFromStats();

    /// <summary>
    /// Get the current value from the Stats component.
    /// Must be implemented by derived classes to provide the correct current value.
    /// </summary>
    /// <returns>The current value for this status bar</returns>
    protected abstract float GetCurrentValueFromStats();

    /// <summary>
    /// Subscribe to relevant stats events.
    /// Must be implemented by derived classes to set up proper event handling.
    /// Example events: health changed, mana changed, etc.
    /// </summary>
    protected abstract void SubscribeToEvents();

    /// <summary>
    /// Unsubscribe from stats events.
    /// Must be implemented by derived classes to clean up event handlers.
    /// Should mirror the subscriptions in SubscribeToEvents.
    /// </summary>
    protected abstract void UnsubscribeFromEvents();

    /// <summary>
    /// Validates required components and configuration.
    /// 
    /// Checks:
    /// - Front bar Image component exists
    /// - Back bar Image component exists
    /// - Chip speed has valid value
    /// 
    /// Logs appropriate error messages if validation fails.
    /// </summary>
    protected virtual void ValidateComponents()
    {
        if (!frontBar)
        {
            Debug.LogError($"[{GetType().Name}] Front bar Image component missing on {gameObject.name}", this);
        }

        if (!backBar)
        {
            Debug.LogError($"[{GetType().Name}] Back bar Image component missing on {gameObject.name}", this);
        }

        if (chipSpeed <= 0)
        {
            Debug.LogWarning($"[{GetType().Name}] Chip speed should be greater than 0 on {gameObject.name}", this);
            chipSpeed = 2f;
        }
    }

    /// <summary>
    /// Initializes stats and subscribes to events.
    /// 
    /// Process:
    /// 1. Finds Stats component in parent hierarchy
    /// 2. Validates Stats component exists
    /// 3. Sets up event subscriptions
    /// 
    /// Logs warning if Stats component not found.
    /// </summary>
    protected virtual void InitializeStats()
    {
        if (!stats)
        {
            stats = GetComponentInParent<Stats>();
            if (!stats)
            {
                Debug.LogWarning($"[{GetType().Name}] Stats component not found on {gameObject.name} or its parents", this);
                return;
            }
        }

        SubscribeToEvents();
    }

    /// <summary>
    /// Updates the maximum value.
    /// 
    /// Features:
    /// - Validates new value is positive
    /// - Updates internal max value
    /// - Logs warning if invalid value provided
    /// </summary>
    /// <param name="newMaxValue">New maximum value to set</param>
    protected virtual void UpdateMaxValue(float newMaxValue)
    {
        if (newMaxValue < 0)
        {
            Debug.LogWarning($"[{GetType().Name}] Attempting to set negative max value on {gameObject.name}", this);
            return;
        }
        maxValue = newMaxValue;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Validates that UI components are properly assigned.
    /// Required for UI updates to function.
    /// </summary>
    private bool ValidateUIComponents()
    {
        if (!frontBar || !backBar)
        {
            Debug.LogWarning($"[{GetType().Name}] UI components missing on {gameObject.name}", this);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Updates the bar when value is being depleted.
    /// 
    /// Process:
    /// 1. Updates front bar immediately to new value
    /// 2. Sets depletion color for visual feedback
    /// 3. Smoothly transitions back bar to match
    /// </summary>
    private void UpdateDepleting(float targetFraction, float currentBackFill, float currentFrontFill)
    {
        frontBar.fillAmount = targetFraction;
        backBar.color = depleteColor;
        
        float percentComplete = CalculateLerpPercentage();
        backBar.fillAmount = Mathf.Lerp(currentBackFill, targetFraction, percentComplete);
    }

    /// <summary>
    /// Updates the bar when value is being restored.
    /// 
    /// Process:
    /// 1. Updates back bar immediately to new value
    /// 2. Sets restoration color for visual feedback
    /// 3. Smoothly transitions front bar to match
    /// </summary>
    private void UpdateRestoring(float targetFraction, float currentFrontFill)
    {
        backBar.color = restoreColor;
        backBar.fillAmount = targetFraction;
        
        float percentComplete = CalculateLerpPercentage();
        frontBar.fillAmount = Mathf.Lerp(currentFrontFill, backBar.fillAmount, percentComplete);
    }

    /// <summary>
    /// Calculates the percentage for lerping based on chip speed.
    /// Uses quadratic easing for smooth transitions.
    /// </summary>
    private float CalculateLerpPercentage()
    {
        lerpTimer += Time.deltaTime;
        float percentComplete = lerpTimer / chipSpeed;
        return percentComplete * percentComplete;
    }

    /// <summary>
    /// Resets the lerp timer for smooth transitions.
    /// Called when value changes to ensure proper animation.
    /// </summary>
    private void ResetLerpTimer()
    {
        lerpTimer = 0f;
    }
    #endregion
}
