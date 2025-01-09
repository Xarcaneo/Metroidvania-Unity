using UnityEngine;
using PixelCrushers.DialogueSystem;

/// <summary>
/// Base class for interactable objects that maintain state and have animations.
/// Provides common functionality for locks and triggers.
/// </summary>
public abstract class InteractableState : Interactable
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Unique ID for this interactable")]
    /// <summary>
    /// Unique identifier for this interactable.
    /// Used to track and persist state between game sessions.
    /// </summary>
    protected string m_stateID;

    [SerializeField]
    [Tooltip("Reference to the animator component")]
    /// <summary>
    /// Reference to the animator component for animations.
    /// Controls feedback animations and state changes.
    /// </summary>
    protected Animator m_animator;
    #endregion

    #region Protected Fields
    /// <summary>
    /// Reference to game events system for state changes.
    /// Cached for efficient access.
    /// </summary>
    protected GameEvents m_gameEvents;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Validates configuration in the Unity Editor.
    /// Ensures critical parameters are properly set.
    /// </summary>
    protected override void OnValidate()
    {
        base.OnValidate();

        if (m_animator == null)
        {
            m_animator = GetComponent<Animator>();
        }
    }

    /// <summary>
    /// Initializes components by caching required references.
    /// Called when the script instance is being loaded.
    /// </summary>
    protected virtual void Awake()
    {
        InitializeComponents();
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Initializes and caches required components.
    /// Called during Awake to ensure early component access.
    /// </summary>
    protected virtual void InitializeComponents()
    {
        if (m_animator == null)
        {
            m_animator = GetComponent<Animator>();
        }

        m_gameEvents = GameEvents.Instance;
        if (m_gameEvents == null)
        {
            Debug.LogWarning($"[{gameObject.name}] GameEvents instance is null!");
        }
    }

    /// <summary>
    /// Validates that all required components are present and properly initialized.
    /// </summary>
    /// <returns>True if all components are valid, false otherwise</returns>
    protected virtual bool ValidateComponents()
    {
        if (m_animator == null)
        {
            Debug.LogError($"[{gameObject.name}] Animator component is missing!");
            return false;
        }

        if (Player.Instance == null)
        {
            Debug.LogError($"[{gameObject.name}] Player instance is null!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Notifies the game system of a state change.
    /// </summary>
    protected virtual void NotifyStateChange()
    {
        if (m_gameEvents != null)
        {
            m_gameEvents.TriggerStateChanged(m_stateID);
        }
    }

    /// <summary>
    /// Called by animation event when animation finishes.
    /// Notifies the interaction system that the interaction is complete.
    /// </summary>
    protected virtual void OnAnimationFinished()
    {
        CallInteractionCompletedEvent();
    }
    #endregion
}
