using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles communication box functionality for dialogue interactions in the game.
/// Manages dialogue state, visual feedback, and interaction state persistence.
/// Integrates with the PixelCrushers Dialogue System for conversation management.
/// </summary>
public class CommunicationBox : Interactable
{
    #region Serialized Fields
    [SerializeField] 
    [Tooltip("ID of the dialogue to play")]
    /// <summary>
    /// ID of the dialogue conversation to play when interacting.
    /// Must match a conversation ID in the Dialogue System database.
    /// Can be "0" for default conversation.
    /// </summary>
    private string dialogue_ID;

    [SerializeField]
    [Tooltip("Unique ID for this communication box")]
    /// <summary>
    /// Unique identifier for this communication box.
    /// Used to track and persist interaction state between game sessions.
    /// Can be "0" for default state.
    /// </summary>
    private string communicationBox_ID;

    [SerializeField]
    [Tooltip("Whether to show the loyal servant image")]
    /// <summary>
    /// Determines if the loyal servant image animation should be played
    /// after the turn on animation completes. Controls visual appearance
    /// of the communication box.
    /// </summary>
    private bool LoyalServantImage = true;
    #endregion

    #region Private Fields
    /// <summary>
    /// Reference to the animator component for visual feedback.
    /// Controls turn on/off and loyal servant animations.
    /// </summary>
    private Animator m_animator;

    /// <summary>
    /// Reference to the dialogue system controller for managing conversations.
    /// Required for starting and managing dialogue interactions.
    /// </summary>
    private DialogueSystemController m_dialogueSystem;

    // Animation state names
    /// <summary>
    /// Constants for animation state names to ensure consistency
    /// and prevent typos in animation calls.
    /// </summary>
    private const string TURN_ON_ANIM = "TurnOn";
    private const string TURN_OFF_ANIM = "TurnOff";
    private const string LOYAL_SERVANT_IDLE_ANIM = "LoyalServantIdle";

    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the communication box by caching required components.
    /// Called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Initializes communication box state after all objects are initialized.
    /// Waits for end of frame to ensure proper initialization order.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution</returns>
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        InitializeState();
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

    /// <summary>
    /// Validates communication box configuration in the Unity Editor.
    /// Ensures critical parameters are properly set.
    /// </summary>
    protected override void OnValidate()
    {
        base.OnValidate();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Handles interaction with the communication box when activated by the player.
    /// Starts the dialogue conversation and manages visual state transitions.
    /// </summary>
    public override void Interact()
    {
        base.Interact();

        m_animator.Play(TURN_ON_ANIM);
        DialogueManager.StartConversation(dialogue_ID);
        SetConversationFinished();
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

        // Get dialogue system
        m_dialogueSystem = FindObjectOfType<DialogueSystemController>();
        if (m_dialogueSystem == null)
        {
            Debug.LogError($"[{gameObject.name}] DialogueSystemController not found in scene!");
        }
    }

    /// <summary>
    /// Initializes communication box state from saved data.
    /// Disables interaction if previously completed.
    /// </summary>
    private void InitializeState()
    {
        try
        {
            var communicationBoxState = DialogueLua.GetVariable("CommunicationBox." + communicationBox_ID).asBool;
            if (communicationBoxState)
            {
                canInteract = false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[{gameObject.name}] Error getting communication box state: {e.Message}");
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
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.onDialogueTrigger += IsInteractionCompleted;
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] GameEvents instance is null!");
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
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.onDialogueTrigger -= IsInteractionCompleted;
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
        return true;
    }

    /// <summary>
    /// Marks the conversation as finished and persists the state.
    /// Updates dialogue system variables and disables further interaction.
    /// </summary>
    private void SetConversationFinished()
    {
        try
        {
            canInteract = false;
            DialogueLua.SetVariable("CommunicationBox." + communicationBox_ID, true);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[{gameObject.name}] Error setting conversation finished: {e.Message}");
        }
    }

    /// <summary>
    /// Handles the completion of dialogue interaction.
    /// Called by the dialogue system when conversation ends.
    /// </summary>
    /// <param name="value">True if dialogue should continue, false if it should end</param>
    protected override void IsInteractionCompleted(bool value)
    {
        if (!value && isInteracting)
        {
            m_animator.Play(TURN_OFF_ANIM);
            isInteracting = false;
        }
    }
    #endregion

    #region Animation Events
    /// <summary>
    /// Called by animation event when turn on animation completes.
    /// Handles transition to loyal servant idle animation if enabled.
    /// </summary>
    public void OnTurnOnAnimationComplete()
    {
        if (LoyalServantImage)
        {
            m_animator.Play(LOYAL_SERVANT_IDLE_ANIM);
        }
    }
    #endregion
}
