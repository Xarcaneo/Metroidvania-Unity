using UnityEngine;

/// <summary>
/// Handles vertical entrance functionality, such as ladder-based scene transitions.
/// Extends the base Entrance class with specific behavior for vertical movement.
/// </summary>
public class VerticalEntrance : Entrance
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("If true, this is a top entrance requiring ladder climb to activate")]
    private bool m_entranceTop = false;
    #endregion

    #region Private Fields
    /// <summary>
    /// Cached reference to GameEvents instance
    /// </summary>
    private GameEvents m_gameEvents;

    /// <summary>
    /// Cached reference to InputManager instance
    /// </summary>
    private InputManager m_inputManager;

    /// <summary>
    /// Cached reference to Player instance
    /// </summary>
    private Player m_player;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the entrance and sets up event handlers
    /// </summary>
    public override void Start()
    {
        base.Start();
        InitializeComponents();

        if (m_entranceTop && ValidateComponents())
        {
            m_gameEvents.onPlayerStateChanged += OnPlayerStateChanged;
            if (scenePortal != null)
            {
                scenePortal.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Cleans up event subscriptions
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();

        if (m_entranceTop && m_gameEvents != null)
        {
            m_gameEvents.onPlayerStateChanged -= OnPlayerStateChanged;
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Handles player entering the entrance trigger
    /// Deactivates player input during transition
    /// </summary>
    public override void EntranceEntered()
    {
        if (!ValidateComponents()) return;

        base.EntranceEntered();

        m_inputManager.isInputActive = false;
        m_gameEvents.DeactivatePlayerInput(true);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initializes and caches required components
    /// </summary>
    private void InitializeComponents()
    {
        m_gameEvents = GameEvents.Instance;
        m_inputManager = InputManager.Instance;
        m_player = Player.Instance;

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
        if (m_gameEvents == null)
        {
            Debug.LogError($"[{gameObject.name}] GameEvents instance is null!");
            return false;
        }

        if (m_inputManager == null)
        {
            Debug.LogError($"[{gameObject.name}] InputManager instance is null!");
            return false;
        }

        if (m_player == null)
        {
            Debug.LogError($"[{gameObject.name}] Player instance is null!");
            return false;
        }

        if (scenePortal == null)
        {
            Debug.LogError($"[{gameObject.name}] Scene portal is null!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Handles player state changes for top entrances
    /// Activates/deactivates scene portal based on ladder climb state
    /// </summary>
    /// <param name="state">Current player state</param>
    private void OnPlayerStateChanged(State state)
    {
        if (!ValidateComponents()) return;

        if (scenePortal != null)
        {
            scenePortal.gameObject.SetActive(state == m_player.LadderClimbState);
        }
    }
    #endregion
}
