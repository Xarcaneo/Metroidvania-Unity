using PixelCrushers;
using UnityEngine;

/// <summary>
/// Handles checkpoint functionality for saving game state and player position.
/// Manages checkpoint activation, player positioning, and game state persistence.
/// Integrates with the SaveSystem for game state management.
/// </summary>
public class Checkpoint : Interactable
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("X offset for player position when activating checkpoint")]
    /// <summary>
    /// Horizontal offset applied to player position when activating checkpoint.
    /// Used to ensure player is properly aligned with the checkpoint visuals.
    /// Default is -0.5f to position player slightly to the left.
    /// </summary>
    private float _playerXOffset = -0.5f;
    #endregion

    #region Private Fields
    /// <summary>
    /// Reference to the animator component for checkpoint animations.
    /// Controls activation and deactivation visual feedback.
    /// </summary>
    private Animator m_animator;

    /// <summary>
    /// Reference to player's movement component for handling orientation.
    /// Used to ensure player faces the correct direction after checkpoint activation.
    /// </summary>
    private Movement m_playerMovement;

    /// <summary>
    /// Reference to player's position saver for checkpoint functionality.
    /// Used to mark this location as the current checkpoint for respawning.
    /// </summary>
    private PlayerPositionSaver m_playerPositionSaver;

    /// <summary>
    /// Reference to the save system for managing game state.
    /// Required for persisting checkpoint and game state.
    /// </summary>
    private SaveSystem m_saveSystem;

    /// <summary>
    /// Reference to game manager for accessing current save slot.
    /// Used to determine which slot to save checkpoint data to.
    /// </summary>
    private GameManager m_gameManager;

    // Animation parameter names
    /// <summary>
    /// Constants for animator parameter names to ensure consistency
    /// and prevent typos in animation calls.
    /// </summary>
    private const string ACTIVATED_PARAM = "activated";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the checkpoint by caching required components.
    /// Called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Initializes player-related components after all objects are initialized.
    /// Called after Awake to ensure player instance exists.
    /// </summary>
    private void Start()
    {
        InitializePlayerComponents();
    }

    /// <summary>
    /// Subscribes to player spawn events when the object becomes enabled.
    /// Ensures component references stay valid after player respawns.
    /// </summary>
    private void OnEnable()
    {
        // Subscribe to player spawn event if needed
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onPlayerSpawned += CachePlayerComponents;
        }
    }

    /// <summary>
    /// Unsubscribes from player spawn events when the object becomes disabled.
    /// Prevents memory leaks and invalid event calls.
    /// </summary>
    private void OnDisable()
    {
        // Unsubscribe from player spawn event
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onPlayerSpawned -= CachePlayerComponents;
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Handles interaction with the checkpoint when activated by the player.
    /// Triggers game saving, positions the player, and activates checkpoint animation.
    /// </summary>
    public override void Interact()
    {
        if (!ValidateComponents()) return;

        // Trigger game saving event
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.GameSaving();
        }

        // Position player
        AlignPlayerWithCheckpoint();

        // Activate checkpoint animation
        m_animator.SetBool(ACTIVATED_PARAM, true);
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

        // Get managers
        m_saveSystem = FindObjectOfType<SaveSystem>();
        if (m_saveSystem == null)
        {
            Debug.LogError($"[{gameObject.name}] SaveSystem not found in scene!");
        }

        m_gameManager = GameManager.Instance;
        if (m_gameManager == null)
        {
            Debug.LogError($"[{gameObject.name}] GameManager instance is null!");
        }
    }

    /// <summary>
    /// Initializes player-related components if player instance exists.
    /// Called during Start after player instantiation.
    /// </summary>
    private void InitializePlayerComponents()
    {
        if (Player.Instance != null)
        {
            CachePlayerComponents();
        }
    }

    /// <summary>
    /// Caches player components for efficient access.
    /// Called when player spawns or during initialization.
    /// </summary>
    private void CachePlayerComponents()
    {
        if (Player.Instance?.Core != null)
        {
            m_playerMovement = Player.Instance.Core.GetCoreComponent<Movement>();
            if (m_playerMovement == null)
            {
                Debug.LogWarning($"[{gameObject.name}] Player Movement component not found!");
            }

            m_playerPositionSaver = Player.Instance.GetComponent<PlayerPositionSaver>();
            if (m_playerPositionSaver == null)
            {
                Debug.LogWarning($"[{gameObject.name}] PlayerPositionSaver component not found!");
            }
        }
    }

    /// <summary>
    /// Validates that all required components are present and properly initialized.
    /// </summary>
    /// <returns>True if all components are valid, false otherwise</returns>
    private bool ValidateComponents()
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

        if (m_playerMovement == null || m_playerPositionSaver == null)
        {
            CachePlayerComponents();
            if (m_playerMovement == null)
            {
                Debug.LogError($"[{gameObject.name}] Player Movement component is missing!");
                return false;
            }
            if (m_playerPositionSaver == null)
            {
                Debug.LogError($"[{gameObject.name}] PlayerPositionSaver component is missing!");
                return false;
            }
        }

        if (m_saveSystem == null)
        {
            Debug.LogError($"[{gameObject.name}] SaveSystem not found!");
            return false;
        }

        if (m_gameManager == null)
        {
            Debug.LogError($"[{gameObject.name}] GameManager instance is null!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Aligns the player's position with the checkpoint.
    /// Applies horizontal offset and maintains vertical position.
    /// </summary>
    private void AlignPlayerWithCheckpoint()
    {
        if (Player.Instance != null)
        {
            Player.Instance.gameObject.transform.position = 
                new Vector3(transform.position.x + _playerXOffset, Player.Instance.gameObject.transform.position.y, 0);
            Player.Instance.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Saves the current checkpoint state to persistent storage.
    /// Updates player position saver and triggers game state save.
    /// </summary>
    private void SaveCheckpoint()
    {
        try
        {
            m_playerPositionSaver.isCheckpoint = true;
            SaveSystem.SaveToSlot(m_gameManager.currentSaveSlot);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[{gameObject.name}] Error saving checkpoint: {e.Message}");
        }
    }
    #endregion

    #region Animation Events
    /// <summary>
    /// Called by animation event when checkpoint activation animation finishes.
    /// Handles player orientation, saves game state, and completes interaction.
    /// </summary>
    public void AnimationFinished()
    {
        if (!ValidateComponents()) return;

        // Handle player facing direction
        if (m_playerMovement.FacingDirection != transform.localScale.x)
        {
            m_playerMovement.Flip();
        }

        // Reactivate player
        Player.Instance.gameObject.SetActive(true);
        Player.Instance.SetPlayerStateToIdle();

        // Save checkpoint
        SaveCheckpoint();

        // Finish interaction
        CallInteractionCompletedEvent();
        m_animator.SetBool(ACTIVATED_PARAM, false);
    }
    #endregion
}
