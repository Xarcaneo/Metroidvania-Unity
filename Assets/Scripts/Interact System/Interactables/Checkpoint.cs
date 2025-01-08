using PixelCrushers;
using UnityEngine;

/// <summary>
/// Handles checkpoint functionality for saving game state and player position
/// </summary>
public class Checkpoint : Interactable
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("X offset for player position when activating checkpoint")]
    private float _playerXOffset = -0.5f;
    #endregion

    #region Private Fields
    private Animator m_animator;
    private Movement m_playerMovement;
    private PlayerPositionSaver m_playerPositionSaver;
    private SaveSystem m_saveSystem;
    private GameManager m_gameManager;

    // Animation parameter names
    private const string ACTIVATED_PARAM = "activated";
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeComponents();
    }

    private void Start()
    {
        InitializePlayerComponents();
    }

    private void OnEnable()
    {
        // Subscribe to player spawn event if needed
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onPlayerSpawned += CachePlayerComponents;
        }
    }

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

    private void InitializePlayerComponents()
    {
        if (Player.Instance != null)
        {
            CachePlayerComponents();
        }
    }

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

    private void AlignPlayerWithCheckpoint()
    {
        if (Player.Instance != null)
        {
            Player.Instance.gameObject.transform.position = 
                new Vector3(transform.position.x + _playerXOffset, Player.Instance.gameObject.transform.position.y, 0);
            Player.Instance.gameObject.SetActive(false);
        }
    }

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
    /// Called by animation event when checkpoint activation animation finishes
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
