using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles area-based event triggers in the game world.
/// Triggers specific events when the player enters the trigger area.
/// </summary>
/// <remarks>
/// Features:
/// - One-shot or repeatable triggers
/// - Dialogue system integration
/// - Automatic player input management
/// - Event-based interaction completion handling
/// - Custom Unity Events support
/// - Optional delay between triggers
/// - Optional cooldown period
/// 
/// Currently supports:
/// - Starting conversations through the Dialogue System
/// - Custom Unity Events
/// - Player input management
/// </remarks>
public class AreaEventTrigger : MonoBehaviour
{
    #region Enums
    /// <summary>
    /// Types of events that can be triggered in the area
    /// </summary>
    private enum EventToTrigger 
    { 
        /// <summary>
        /// Triggers a dialogue conversation
        /// </summary>
        StartConversation,
        
        /// <summary>
        /// Triggers a custom Unity Event
        /// </summary>
        CustomEvent
    }
    #endregion

    #region Serialized Fields
    [Header("Trigger Settings")]
    
    /// <summary>
    /// The type of event to trigger when player enters the area
    /// </summary>
    [SerializeField]
    [Tooltip("Select which event should be triggered when player enters this area")]
    private EventToTrigger eventToTrigger;

    /// <summary>
    /// Whether the trigger should be destroyed after first use
    /// </summary>
    [SerializeField]
    [Tooltip("If true, the trigger will be destroyed after first use")]
    private bool isOneShot = true;

    /// <summary>
    /// Delay before the trigger can be activated again
    /// </summary>
    [SerializeField]
    [Tooltip("Time in seconds before the trigger can be activated again (0 for no cooldown)")]
    private float cooldownTime = 0f;

    /// <summary>
    /// Whether to disable player input during the event
    /// </summary>
    [SerializeField]
    [Tooltip("If true, player input will be disabled during the event")]
    private bool disablePlayerInput = true;

    [Header("Event Settings")]
    
    /// <summary>
    /// Variable name for the event (e.g., conversation name for dialogue)
    /// </summary>
    [SerializeField]
    [Tooltip("Name of the conversation or other string identifier needed by the event")]
    private string stringVariable;

    /// <summary>
    /// Custom Unity Event to trigger
    /// </summary>
    [SerializeField]
    [Tooltip("Custom Unity Event to trigger (only used with CustomEvent type)")]
    private UnityEvent onTriggerEvent;
    #endregion

    #region Private Fields
    /// <summary>
    /// Whether the trigger is currently in cooldown
    /// </summary>
    private bool isInCooldown = false;

    /// <summary>
    /// Timer for tracking cooldown
    /// </summary>
    private float cooldownTimer = 0f;
    #endregion

    #region Event Subscriptions
    /// <summary>
    /// Subscribes to necessary game events when enabled
    /// </summary>
    private void OnEnable()
    {
        try
        {
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.onDialogueTrigger += OnInteractionCompleted;
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] GameEvents.Instance is null. Event subscription failed.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[{gameObject.name}] Error during event subscription: {e.Message}");
        }
    }

    /// <summary>
    /// Unsubscribes from game events when disabled
    /// </summary>
    private void OnDisable()
    {
        try
        {
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.onDialogueTrigger -= OnInteractionCompleted;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[{gameObject.name}] Error during event unsubscription: {e.Message}");
        }
    }
    #endregion

    #region Update Logic
    /// <summary>
    /// Handles cooldown timer update
    /// </summary>
    private void Update()
    {
        if (isInCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isInCooldown = false;
            }
        }
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Handles the completion of an interaction
    /// </summary>
    /// <param name="completed">Whether the interaction was completed successfully</param>
    private void OnInteractionCompleted(bool completed)
    {
        if (disablePlayerInput)
        {
            GameEvents.Instance?.DeactivatePlayerInput(false);
        }
    }

    /// <summary>
    /// Handles trigger area entry by the player
    /// </summary>
    /// <param name="collision">The collider that entered the trigger area</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || isInCooldown)
            return;

        HandleTriggerEvent();
        
        if (cooldownTime > 0f)
        {
            isInCooldown = true;
            cooldownTimer = cooldownTime;
        }

        if (isOneShot)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Event Implementation
    /// <summary>
    /// Handles the execution of the selected trigger event
    /// </summary>
    private void HandleTriggerEvent()
    {
        if (disablePlayerInput)
        {
            GameEvents.Instance?.DeactivatePlayerInput(true);
        }

        switch (eventToTrigger)
        {
            case EventToTrigger.StartConversation:
                if (string.IsNullOrEmpty(stringVariable))
                {
                    Debug.LogWarning($"[{gameObject.name}] Conversation name is empty!");
                    return;
                }
                DialogueManager.StartConversation(stringVariable);
                break;

            case EventToTrigger.CustomEvent:
                onTriggerEvent?.Invoke();
                break;

            default:
                Debug.LogWarning($"[{gameObject.name}] Unhandled event type: {eventToTrigger}");
                break;
        }
    }
    #endregion
}
