using PixelCrushers.DialogueSystem;
using UnityEngine;

/// <summary>
/// Handles NPC (Non-Player Character) functionality for dialogue interactions.
/// Manages dialogue triggering and completion through the Dialogue System integration.
/// Provides event-based interaction completion handling.
/// </summary>
public class NPC : Interactable
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("ID of the dialogue conversation to start when interacting")]
    /// <summary>
    /// ID of the dialogue conversation to play when interacting with the NPC.
    /// Must match a conversation ID in the Dialogue System database.
    /// </summary>
    private string m_dialogueID;
    #endregion

    #region Private Fields
    /// <summary>
    /// Reference to game events system for dialogue completion handling.
    /// Cached for efficient access and cleanup.
    /// </summary>
    private GameEvents m_gameEvents;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Validates NPC configuration in the Unity Editor.
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
    /// Initializes the NPC by caching required components.
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
    /// Handles interaction with the NPC when activated by the player.
    /// Starts the dialogue conversation using the configured dialogue ID.
    /// </summary>
    public override void Interact()
    {
        if (!ValidateComponents()) return;
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
        if (!value)
        {
            CallInteractionCompletedEvent();
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
    /// Validates that all required components and parameters are properly set.
    /// </summary>
    /// <returns>True if all components are valid, false otherwise</returns>
    private bool ValidateComponents()
    {
        if (m_dialogueID == null)
        {
            Debug.LogError($"[{gameObject.name}] Dialogue ID is not set!");
            return false;
        }

        if (m_gameEvents == null)
        {
            Debug.LogWarning($"[{gameObject.name}] GameEvents instance is null!");
            // Don't return false as GameEvents is not critical for basic dialogue
        }

        return true;
    }
    #endregion
}
