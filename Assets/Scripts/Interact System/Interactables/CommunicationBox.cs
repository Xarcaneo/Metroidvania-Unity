using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles communication box functionality for dialogue interactions
/// </summary>
public class CommunicationBox : Interactable
{
    #region Serialized Fields
    [SerializeField] 
    [Tooltip("ID of the dialogue to play")]
    private string dialogue_ID;

    [SerializeField]
    [Tooltip("Unique ID for this communication box")]
    private string communicationBox_ID;

    [SerializeField]
    [Tooltip("Whether to show the loyal servant image")]
    private bool LoyalServantImage = true;
    #endregion

    #region Private Fields
    private Animator m_animator;
    private DialogueSystemController m_dialogueSystem;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeComponents();
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        InitializeState();
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        
        if (string.IsNullOrWhiteSpace(dialogue_ID) && dialogue_ID != "0")
        {
            Debug.LogWarning($"[{gameObject.name}] Dialogue ID is not set!");
        }

        if (string.IsNullOrWhiteSpace(communicationBox_ID) && communicationBox_ID != "0")
        {
            Debug.LogWarning($"[{gameObject.name}] Communication Box ID is not set!");
        }
    }
    #endregion

    #region Public Methods
    public override void Interact()
    {
        if (!ValidateComponents()) return;

        base.Interact();

        m_animator.Play("TurnOn");
        DialogueManager.StartConversation(dialogue_ID);
        SetConversationFinished();
    }
    #endregion

    #region Private Methods
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

    private void InitializeState()
    {
        if (!ValidateComponents()) return;

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

    private bool ValidateComponents()
    {
        if (m_animator == null)
        {
            Debug.LogError($"[{gameObject.name}] Animator component is missing!");
            return false;
        }

        if (m_dialogueSystem == null)
        {
            Debug.LogError($"[{gameObject.name}] DialogueSystemController not found!");
            return false;
        }

        if (string.IsNullOrWhiteSpace(dialogue_ID) && dialogue_ID != "0")
        {
            Debug.LogError($"[{gameObject.name}] Dialogue ID is not set!");
            return false;
        }

        if (string.IsNullOrWhiteSpace(communicationBox_ID) && communicationBox_ID != "0")
        {
            Debug.LogError($"[{gameObject.name}] Communication Box ID is not set!");
            return false;
        }

        return true;
    }

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
    #endregion

    #region Protected Methods
    protected override void IsInteractionCompleted(bool value)
    {
        if (!ValidateComponents()) return;

        if (!value && isInteracting)
        {
            m_animator.Play("TurnOff");
            isInteracting = false;
        }
    }
    #endregion

    #region Animation Events
    /// <summary>
    /// Called by animation event when turn on animation completes
    /// </summary>
    public void OnTurnOnAnimationComplete()
    {
        if (!ValidateComponents()) return;

        if (LoyalServantImage)
        {
            m_animator.Play("LoyalServantIdle");
        }
    }
    #endregion
}
