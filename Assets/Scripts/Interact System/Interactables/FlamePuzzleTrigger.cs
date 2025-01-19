using PixelCrushers.DialogueSystem;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles flame puzzle trigger functionality.
/// Manages puzzle state, animations, and connected gate interactions.
/// </summary>
public class FlamePuzzleTrigger : InteractableState
{
    #region Serialized Fields
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
    protected override void Awake()
    {
        base.Awake();
        InitializeComponents();
    }

    /// <summary>
    /// Updates trigger animations based on state.
    /// </summary>
    /// <param name="state">Current trigger state</param>
    protected override void UpdateVisuals(bool state) => m_animator.SetBool(COMPLETED_PARAM, state);

    /// <summary>
    /// Called after state is initialized from Lua.
    /// Override this to customize behavior when state is initialized.
    /// </summary>
    /// <param name="state">The initialized state value</param>
    protected override void OnStateInitialized(bool state)
    {
        base.OnStateInitialized(state);
        isCompleted = state;
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
    /// Handles scene loading events
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == PUZZLE_SCENE && mode == LoadSceneMode.Additive && isInteracting)
        {
            var puzzleManager = FindObjectOfType<FlamePuzzleManager>();
            if (puzzleManager != null)
            {
                puzzleManager.InstantiateObject(m_puzzleID, PUZZLE_SCENE);
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
    /// Unlocks the lock and notifies connected gates
    /// </summary>
    private void UnlockAndNotify()
    {
        canInteract = false; // Disable further interaction
        UpdateState(true);
    }

    /// <summary>
    /// Handles puzzle completion
    /// </summary>
    private void OnPuzzleCompleted()
    {
        UnlockAndNotify();

        isCompleted = true;

        GameEvents.Instance.DeactivatePlayerInput(false);
        GameEvents.Instance.PuzzleClose(PUZZLE_SCENE);
    }
    #endregion
}
