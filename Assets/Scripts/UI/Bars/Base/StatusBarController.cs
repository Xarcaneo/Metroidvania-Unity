using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Base class for status bars (health, mana, etc.) that provides common functionality
/// for displaying and updating UI bars with smooth transitions.
/// </summary>
public abstract class StatusBarController : MonoBehaviour
{
    #region Serialized Fields
    [Header("UI Components")]
    [Tooltip("The front fill image of the status bar")]
    [SerializeField] protected Image frontBar;
    [Tooltip("The back fill image of the status bar that shows transitions")]
    [SerializeField] protected Image backBar;

    [Header("Settings")]
    [Tooltip("Speed at which the bar transitions when value changes")]
    [SerializeField] protected float chipSpeed = 2f;
    #endregion

    #region Protected Fields
    protected float currentValue;
    protected float maxValue = 100f;
    protected float lerpTimer;
    protected Stats stats;

    protected Color depleteColor = Color.white;
    protected Color restoreColor = Color.white;
    #endregion

    #region Unity Methods
    protected virtual void Awake()
    {
        ValidateComponents();
    }

    protected virtual void Start()
    {
        InitializeStats();
    }

    protected virtual void Update()
    {
        if (!stats) return;

        UpdateMaxValue(GetMaxValueFromStats());
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        UpdateUI();
    }

    public virtual void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Updates the status bar UI with smooth transitions.
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
    /// </summary>
    public virtual void ReduceValue(float amount)
    {
        currentValue = Mathf.Max(0, currentValue - amount);
        ResetLerpTimer();
    }

    /// <summary>
    /// Restores the value by the specified amount.
    /// </summary>
    public virtual void RestoreValue(float amount)
    {
        currentValue = Mathf.Min(maxValue, currentValue + amount);
        ResetLerpTimer();
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Get the maximum value from the Stats component.
    /// </summary>
    protected abstract float GetMaxValueFromStats();

    /// <summary>
    /// Subscribe to relevant stats events.
    /// </summary>
    protected abstract void SubscribeToEvents();

    /// <summary>
    /// Unsubscribe from stats events.
    /// </summary>
    protected abstract void UnsubscribeFromEvents();

    /// <summary>
    /// Validates required components and configuration.
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
    /// </summary>
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
    /// </summary>
    private float CalculateLerpPercentage()
    {
        lerpTimer += Time.deltaTime;
        float percentComplete = lerpTimer / chipSpeed;
        return percentComplete * percentComplete;
    }

    /// <summary>
    /// Resets the lerp timer for smooth transitions.
    /// </summary>
    private void ResetLerpTimer()
    {
        lerpTimer = 0f;
    }
    #endregion
}
