using Com.LuisPedroFonseca.ProCamera2D;
using PixelCrushers;
using UnityEngine;

/// <summary>
/// Handles portal functionality for scene transitions.
/// Manages portal animations, player state, and scene loading through ScenePortal integration.
/// Controls input state during transition to ensure smooth scene changes.
/// </summary>
public class Portal : Interactable
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Reference to the ScenePortal component that handles the actual scene transition")]
    /// <summary>
    /// ScenePortal component that manages the scene transition logic.
    /// Must be assigned in inspector. Handles destination scene loading and player positioning.
    /// </summary>
    private ScenePortal m_scenePortal;
    #endregion

    #region Private Fields
    /// <summary>
    /// Reference to the animator component for portal animations.
    /// Controls portal activation and transition effects.
    /// </summary>
    private Animator m_animator;

    /// <summary>
    /// Reference to the input manager for controlling player input during transitions.
    /// Cached for efficient access and cleanup.
    /// </summary>
    private InputManager m_inputManager;

    /// <summary>
    /// Reference to game events system for player input control.
    /// Cached for efficient access.
    /// </summary>
    private GameEvents m_gameEvents;

    // Animation parameter names
    /// <summary>
    /// Constants for animator parameter names to ensure consistency
    /// and prevent typos in animation calls.
    /// </summary>
    private const string ACTIVATED_PARAM = "activated";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the portal by caching required components.
    /// Called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Cleanup when portal is destroyed. Ensures input is re-enabled.
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (m_inputManager != null)
        {
            m_inputManager.isInputActive = true;
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Handles interaction with the portal when activated by the player.
    /// Disables player input and starts portal activation animation.
    /// </summary>
    public override void Interact()
    {
        if (!ValidateComponents()) return;

        // Disable input during transition
        m_inputManager.isInputActive = false;
        m_gameEvents.DeactivatePlayerInput(true);

        // Start portal animation
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
        m_inputManager = InputManager.Instance;
        if (m_inputManager == null)
        {
            Debug.LogError($"[{gameObject.name}] InputManager instance is null!");
        }

        m_gameEvents = GameEvents.Instance;
        if (m_gameEvents == null)
        {
            Debug.LogError($"[{gameObject.name}] GameEvents instance is null!");
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

        if (m_inputManager == null)
        {
            Debug.LogError($"[{gameObject.name}] InputManager instance is null!");
            return false;
        }

        if (m_gameEvents == null)
        {
            Debug.LogError($"[{gameObject.name}] GameEvents instance is null!");
            return false;
        }

        if (m_scenePortal == null)
        {
            Debug.LogError($"[{gameObject.name}] ScenePortal component is not assigned!");
            return false;
        }

        return true;
    }
    #endregion

    #region Animation Events
    /// <summary>
    /// Called by animation event when portal activation animation finishes.
    /// Triggers the actual scene transition through ScenePortal.
    /// </summary>
    public void AnimationFinished()
    {
        if (!ValidateComponents()) return;
        m_scenePortal.UsePortal();
    }

    /// <summary>
    /// Called by animation event during portal animation.
    /// Hides the player character during transition.
    /// </summary>
    public void AnimationTriggerEvent()
    {
        if (Player.Instance != null)
        {
            Player.Instance.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] Player instance is null during portal animation!");
        }
    }
    #endregion
}
