using PixelCrushers.DialogueSystem;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles flame puzzle trigger functionality.
/// Manages puzzle state, animations, and connected gate interactions.
/// </summary>
public class FlamePuzzleTrigger : Interactable
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Unique ID for this flame trigger")]
    /// <summary>
    /// Unique identifier for this flame trigger.
    /// Used to track and persist trigger state between game sessions.
    /// </summary>
    private int m_triggerID = 1;

    [SerializeField]
    [Tooltip("ID for the puzzle instance")]
    /// <summary>
    /// Identifier for the specific puzzle instance.
    /// Used to track which puzzle configuration to load.
    /// </summary>
    private int m_puzzleID = 0;

    private bool isCompleted = false;
    #endregion

    #region Private Fields
    /// <summary>
    /// Reference to the animator component for flame animations.
    /// Controls flame on/off animations.
    /// </summary>
    private Animator m_animator;

    /// <summary>
    /// Reference to game events system for state changes.
    /// Cached for efficient access.
    /// </summary>
    private GameEvents m_gameEvents;

    // Animation parameter names
    private const string COMPLETED_PARAM = "isCompleted";

    // Scene name constant
    private const string PUZZLE_SCENE = "Flame Puzzle";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Subscribe to scene loading events
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    /// <summary>
    /// Unsubscribe from scene loading events
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    /// <summary>
    /// Initializes the flame trigger by caching required components.
    /// Called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Initializes flame trigger state after all objects are initialized.
    /// Checks if trigger was previously activated.
    /// </summary>
    private IEnumerator Start()
    {
        m_animator = GetComponent<Animator>();
        yield return new WaitForEndOfFrame();
        var triggerState = DialogueLua.GetVariable("FlamePuzzle." + m_triggerID).asBool;
        if (triggerState) SetCompleted();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Handles interaction with the flame trigger when activated by the player.
    /// Opens the flame puzzle minigame scene.
    /// </summary>
    public override void Interact()
    {
        base.Interact();
        GameEvents.Instance.DeactivatePlayerInput(true);
        GameEvents.Instance.PuzzleOpen(PUZZLE_SCENE);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initializes and caches required components.
    /// </summary>
    private void InitializeComponents()
    {
        if (m_animator == null)
        {
            m_animator = GetComponent<Animator>();
        }

        m_gameEvents = GameEvents.Instance;
        if (m_gameEvents == null)
        {
            Debug.LogWarning($"[{gameObject.name}] GameEvents instance is null!");
        }
    }

    /// <summary>
    /// Sets the trigger to completed state
    /// </summary>
    private void SetCompleted()
    {
        canInteract = false;
        isCompleted = true;
        m_animator.SetBool(COMPLETED_PARAM, true);
    }

    /// <summary>
    /// Handles scene loading events
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == PUZZLE_SCENE && mode == LoadSceneMode.Additive && isInteracting)
        {
            var puzzleManager = FindObjectOfType<FlamePuzzleManager>();
            if (puzzleManager != null)
            {
                puzzleManager.InstantiateObject(m_triggerID, m_puzzleID, PUZZLE_SCENE);
                FlamePuzzleManager.Instance.PuzzleCompleted += OnPuzzleCompleted;
            }
        }
    }

    /// <summary>
    /// Handles scene unloading events
    /// </summary>
    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == PUZZLE_SCENE)
        {
            GameEvents.Instance.DeactivatePlayerInput(false);

            if (FlamePuzzleManager.Instance)
                FlamePuzzleManager.Instance.PuzzleCompleted -= OnPuzzleCompleted;

            if (!isCompleted)
                CallInteractionCompletedEvent();
        }
    }

    /// <summary>
    /// Handles puzzle completion
    /// </summary>
    private void OnPuzzleCompleted()
    {
        SetCompleted();
        DialogueLua.SetVariable("FlamePuzzle." + m_triggerID, true);
        GameEvents.Instance.DeactivatePlayerInput(false);
        GameEvents.Instance.PuzzleClose(PUZZLE_SCENE);
    }
    #endregion
}
