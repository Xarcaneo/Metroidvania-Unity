using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Controls a platform that players can drop through by pressing down + jump.
/// Manages collision states and player interactions with the platform.
/// </summary>
public class Platform : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Time in seconds before restoring platform collision after drop-through")]
    [Range(0.1f, 2f)]
    private float m_timeToRevertCollision = 0.5f;
    #endregion

    #region Private Fields
    /// <summary>
    /// Reference to the player's input handler
    /// </summary>
    private PlayerInputHandler m_playerInputHandler;

    /// <summary>
    /// Reference to the platform effector component
    /// </summary>
    private PlatformEffector2D m_effector;

    /// <summary>
    /// Flag indicating if player is currently on the platform
    /// </summary>
    private bool m_playerOnPlatform;

    // Layer constants
    private const string PLAYER_LAYER = "Player";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes required components
    /// </summary>
    private void Start()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Updates player input handling
    /// </summary>
    private void Update()
    {
        HandlePlayerInput();
    }

    /// <summary>
    /// Handles collision exit events
    /// </summary>
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(PLAYER_LAYER))
        {
            m_playerOnPlatform = false;
        }
    }

    /// <summary>
    /// Handles collision enter events
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(PLAYER_LAYER))
        {
            m_playerOnPlatform = true;
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initializes and caches required components
    /// </summary>
    private void InitializeComponents()
    {
        m_effector = GetComponent<PlatformEffector2D>();
        if (!ValidateComponents())
        {
            Debug.LogError($"[{gameObject.name}] Failed to initialize required components!");
        }
    }

    /// <summary>
    /// Validates that all required components are present
    /// </summary>
    /// <returns>True if all components are valid, false otherwise</returns>
    private bool ValidateComponents()
    {
        if (m_effector == null)
        {
            Debug.LogError($"[{gameObject.name}] PlatformEffector2D component is missing!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Handles player input for platform interactions
    /// </summary>
    private void HandlePlayerInput()
    {
        // Try to find player input handler if not already cached
        if (m_playerInputHandler == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_playerInputHandler = player.GetComponent<PlayerInputHandler>();
            }
            return;
        }

        // Check for crouch-jump input
        float yInput = m_playerInputHandler.NormInputY;
        bool jumpInput = m_playerInputHandler.JumpInput;

        if (jumpInput && yInput == -1)
        {
            OnPlayerCrouchJump();
        }
    }

    /// <summary>
    /// Handles player crouch-jump through platform
    /// </summary>
    private void OnPlayerCrouchJump()
    {
        if (!m_playerOnPlatform || !ValidateComponents()) return;

        // Disable collision with player
        m_effector.colliderMask &= ~(1 << LayerMask.NameToLayer(PLAYER_LAYER));
        
        // Disable player jump temporarily
        if (Player.Instance != null)
        {
            Player.Instance.JumpState.canJump = false;
        }

        // Start collision revert timer
        StartCoroutine(RevertCollisionCoroutine());
    }

    /// <summary>
    /// Coroutine to revert platform collision after delay
    /// </summary>
    private IEnumerator RevertCollisionCoroutine()
    {
        if (!ValidateComponents()) yield break;

        yield return new WaitForSeconds(m_timeToRevertCollision);

        // Re-enable player jump
        if (Player.Instance != null)
        {
            Player.Instance.JumpState.canJump = true;
        }

        // Re-enable collision with player
        m_effector.colliderMask |= (1 << LayerMask.NameToLayer(PLAYER_LAYER));
    }
    #endregion
}
