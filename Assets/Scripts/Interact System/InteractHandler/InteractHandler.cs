using UnityEngine;

/// <summary>
/// Handles player interactions with interactable objects in the game world.
/// Manages interaction states, pictogram display, and player proximity detection.
/// </summary>
/// <remarks>
/// Features:
/// - Player proximity detection
/// - Interaction state management
/// - Pictogram display control
/// - Item detection integration
/// - Event-based interaction handling
/// 
/// Usage:
/// Attach this component to objects that need to handle player interactions
/// and display pictograms based on interaction availability.
/// </remarks>
public class InteractHandler : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>
    /// Reference to the interactable object component
    /// </summary>
    [SerializeField]
    [Tooltip("The Interactable component that defines the interaction behavior")]
    protected Interactable interactableObject;

    /// <summary>
    /// Reference to the pictogram handler for UI feedback
    /// </summary>
    [SerializeField]
    [Tooltip("The PictogramHandler component for displaying interaction indicators")]
    protected PictogramHandler pictogramHandler;
    #endregion

    #region Private Fields
    /// <summary>
    /// Collider for interaction trigger area
    /// </summary>
    private Collider2D m_collider;

    /// <summary>
    /// Cached reference to the player's ItemDetector
    /// </summary>
    private ItemDetector m_itemDetector;

    /// <summary>
    /// Whether the player is within interaction range
    /// </summary>
    protected bool playerInRange = false;

    /// <summary>
    /// Whether interaction is currently allowed
    /// </summary>
    private bool canInteract = true;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes required components
    /// </summary>
    private void Start()
    {
        ValidateComponents();
        m_collider = GetComponent<Collider2D>();
        
        // Cache the ItemDetector reference
        if (Player.Instance?.Core != null)
        {
            m_itemDetector = Player.Instance.Core.GetCoreComponent<ItemDetector>();
            if (m_itemDetector == null)
            {
                Debug.LogWarning($"[{gameObject.name}] Player's ItemDetector component not found!");
            }
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] Player instance or Core not found!");
        }
    }

    /// <summary>
    /// Subscribes to necessary events
    /// </summary>
    private void OnEnable()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onPlayerInteractTrigger += HandlePlayerInteract;
        }

        if (interactableObject != null)
        {
            interactableObject.onInteractionCompleted += HandleInteractionCompleted;
        }
    }

    /// <summary>
    /// Unsubscribes from events and cleans up
    /// </summary>
    private void OnDisable()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onPlayerInteractTrigger -= HandlePlayerInteract;
        }

        if (interactableObject != null)
        {
            interactableObject.onInteractionCompleted -= HandleInteractionCompleted;
        }

        // Ensure we unsubscribe from ItemDetector if it exists
        if (m_itemDetector != null)
        {
            m_itemDetector.onItemDetected -= HandleItemDetection;
        }

        StopAllCoroutines();
    }

    /// <summary>
    /// Validates component setup in editor
    /// </summary>
    private void OnValidate()
    {
        // Only validate if the component is active in the scene
        if (!gameObject.activeInHierarchy || !enabled)
            return;

        #if UNITY_EDITOR
        // Optional: Only show warnings if we're in play mode or the component is active
        if (UnityEditor.EditorApplication.isPlaying || UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        {
            ValidateReferences();
        }
        #endif
    }

    /// <summary>
    /// Validates that all required references are assigned
    /// </summary>
    private void ValidateReferences()
    {
        if (interactableObject == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Interactable object reference is missing!");
        }

        if (pictogramHandler == null)
        {
            Debug.LogWarning($"[{gameObject.name}] PictogramHandler reference is missing!");
        }
    }

    /// <summary>
    /// Validates that all required components are present
    /// </summary>
    private void ValidateComponents()
    {
        ValidateReferences();

        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogError($"[{gameObject.name}] Collider2D component is missing!");
        }
    }
    #endregion

    #region Interaction Handling
    /// <summary>
    /// Handles player interaction input
    /// </summary>
    /// <param name="isInteracting">Whether the player is attempting to interact</param>
    private void HandlePlayerInteract(bool isInteracting)
    {
        if (!CanProcessInteraction(isInteracting))
            return;

        // Process interaction
        interactableObject.canInteract = false;
        pictogramHandler?.HidePictogram();
        interactableObject.Interact();
    }

    /// <summary>
    /// Handles completion of an interaction
    /// </summary>
    private void HandleInteractionCompleted()
    {
        if (interactableObject != null)
        {
            interactableObject.canInteract = true;
            UpdatePictogramVisibility();
        }
    }

    /// <summary>
    /// Updates the pictogram visibility based on current state
    /// </summary>
    protected void UpdatePictogramVisibility()
    {
        if (pictogramHandler == null) return;

        if (ShouldShowPictogram())
        {
            pictogramHandler.ShowPictogram(0);
        }
    }

    /// <summary>
    /// Handles item detection state changes
    /// </summary>
    /// <param name="isItemDetected">Whether an item is detected</param>
    private void HandleItemDetection(bool isItemDetected)
    {
        if (pictogramHandler == null) return;

        canInteract = !isItemDetected;
        
        if (isItemDetected)
        {
            pictogramHandler.HidePictogram();
        }
        else
        {
            UpdatePictogramVisibility();
        }
    }
    #endregion

    #region Trigger Handling
    /// <summary>
    /// Handles player entering the interaction zone
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (m_itemDetector != null)
        {
            m_itemDetector.onItemDetected += HandleItemDetection;
            playerInRange = true;

            // Check initial item detection state
            HandleItemDetection(m_itemDetector.itemsDetected > 0);
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] ItemDetector not found on player!");
        }
    }

    /// <summary>
    /// Handles player leaving the interaction zone
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (m_itemDetector != null)
        {
            m_itemDetector.onItemDetected -= HandleItemDetection;
        }

        playerInRange = false;
        pictogramHandler?.HidePictogram();
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Checks if interaction can be processed
    /// </summary>
    /// <param name="isInteracting">Whether the player is attempting to interact</param>
    /// <returns>True if interaction can proceed</returns>
    private bool CanProcessInteraction(bool isInteracting)
    {
        if (!playerInRange || !isInteracting || !canInteract || 
            interactableObject == null || !interactableObject.canInteract)
            return false;

        var player = Player.Instance;
        if (player == null || player.StateMachine == null || 
            player.StateMachine.CurrentState != player.IdleState)
            return false;

        return true;
    }

    /// <summary>
    /// Determines if the pictogram should be shown
    /// </summary>
    /// <returns>True if the pictogram should be visible</returns>
    private bool ShouldShowPictogram()
    {
        return playerInRange && 
               interactableObject != null && 
               interactableObject.canInteract && 
               canInteract;
    }
    #endregion
}