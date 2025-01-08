using PixelCrushers.DialogueSystem;
using UnityEngine;

/// <summary>
/// Handles lectern functionality for displaying text-based information.
/// Manages lectern animations, dialogue triggering, and interaction state.
/// Integrates with the Dialogue System for text display.
/// </summary>
public class Lectern : Interactable
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("ID of the dialogue to display when interacting")]
    /// <summary>
    /// ID of the dialogue conversation to play when interacting.
    /// Must match a conversation ID in the Dialogue System database.
    /// </summary>
    private string m_dialogueID;
    #endregion

    #region Private Fields
    /// <summary>
    /// Reference to the animator component for lectern animations.
    /// Controls open/close animations when showing/hiding text.
    /// </summary>
    private Animator m_animator;

    /// <summary>
    /// Reference to game events system for dialogue completion handling.
    /// Cached for efficient access and cleanup.
    /// </summary>
    private GameEvents m_gameEvents;

    // Animation state names
    /// <summary>
    /// Constants for animation state names to ensure consistency
    /// and prevent typos in animation calls.
    /// </summary>
    private const string OPEN_ANIM = "Open";
    private const string CLOSE_ANIM = "Close";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Validates lectern configuration in the Unity Editor.
    /// Ensures critical parameters are properly set.
    /// </summary>
    protected override void OnValidate()
    {
        base.OnValidate();

        if (m_dialogueID == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Dialogue ID is not set!");
        }
    }

    /// <summary>
    /// Initializes the lectern by caching required components.
    /// Called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Subscribes to dialogue events when the object becomes enabled.
    /// Ensures proper event handling for dialogue completion.
    /// </summary>
    private void OnEnable()
    {
        SubscribeToEvents();
    }

    /// <summary>
    /// Unsubscribes from dialogue events when the object becomes disabled.
    /// Prevents memory leaks and invalid event calls.
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Handles interaction with the lectern when activated by the player.
    /// Opens the lectern and starts the dialogue conversation.
    /// </summary>
    public override void Interact()
    {
        if (!ValidateComponents()) return;

        base.Interact();
        m_animator.Play(OPEN_ANIM);
        DialogueManager.StartConversation(m_dialogueID);
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Handles the completion of dialogue interaction.
    /// Called by the dialogue system when conversation ends.
    /// </summary>
    /// <param name="value">True if dialogue should continue, false if it should end</param>
    protected override void IsInteractionCompleted(bool value)
    {
        if (!value && isInteracting)
        {
            m_animator.Play(CLOSE_ANIM);
            isInteracting = false;
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initializes and caches required components.
    /// Called during Awake to ensure early component access.
    /// </summary>
    private void InitializeComponents()
    {
        // Get animator
        m_animator = GetComponent<Animator>();
        if (m_animator == null)
        {
            Debug.LogError($"[{gameObject.name}] Animator component is missing!");
        }

        // Get game events
        m_gameEvents = GameEvents.Instance;
        if (m_gameEvents == null)
        {
            Debug.LogWarning($"[{gameObject.name}] GameEvents instance is null!");
        }
    }

    /// <summary>
    /// Subscribes to dialogue system events.
    /// Sets up event handling for dialogue completion.
    /// </summary>
    private void SubscribeToEvents()
    {
        try
        {
            if (m_gameEvents != null)
            {
                m_gameEvents.onDialogueTrigger += IsInteractionCompleted;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[{gameObject.name}] Error subscribing to events: {e.Message}");
        }
    }

    /// <summary>
    /// Unsubscribes from dialogue system events.
    /// Cleans up event handlers to prevent memory leaks.
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        try
        {
            if (m_gameEvents != null)
            {
                m_gameEvents.onDialogueTrigger -= IsInteractionCompleted;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[{gameObject.name}] Error unsubscribing from events: {e.Message}");
        }
    }

    /// <summary>
    /// Validates that all required components are present and properly initialized.
    /// </summary>
    /// <returns>True if all components are valid, false otherwise</returns>
    private bool ValidateComponents()
    {
        if (string.IsNullOrWhiteSpace(m_dialogueID))
        {
            Debug.LogError($"[{gameObject.name}] Dialogue ID is not set!");
            return false;
        }

        if (m_animator == null)
        {
            Debug.LogError($"[{gameObject.name}] Animator component is missing!");
            return false;
        }

        return true;
    }
    #endregion

    #region Animation Events
    /// <summary>
    /// Called by animation event when lectern animation finishes.
    /// Notifies the interaction system that the interaction is complete.
    /// </summary>
    public void AnimationFinished()
    {
        CallInteractionCompletedEvent();
    }
    #endregion
}
