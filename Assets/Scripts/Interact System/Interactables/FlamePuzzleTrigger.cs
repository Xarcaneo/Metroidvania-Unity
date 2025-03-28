using PixelCrushers.DialogueSystem;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityCore.GameManager;
using Menu;

/// <summary>
/// Handles flame puzzle trigger functionality.
/// Manages puzzle state, animations, and connected gate interactions.
/// </summary>
public class FlamePuzzleTrigger : InteractableState
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("IDs for puzzle instances that need to be loaded")]
    /// <summary>
    /// Array of puzzle instance IDs.
    /// Used to track which puzzle configurations to load.
    /// Useful when a single trigger needs to load multiple related puzzles.
    /// </summary>
    private int[] m_puzzleIDs = new int[] { 0 };

    private bool isCompleted = false;
    #endregion

    #region Private Fields
    // Animation parameter names
    private const string COMPLETED_PARAM = "isCompleted";

    // Scene name constant
    private const string PUZZLE_SCENE = "Flame Puzzle";
    #endregion

    #region Unity Lifecycle
    private void OnEnable()
    {
        GameMenu.GameState.OnMinigameSetup += OnMinigameSetup;
        GameMenu.GameState.OnMinigameCancelled += OnMinigameCancelled;
    }

    private void OnDisable()
    {
        GameMenu.GameState.OnMinigameSetup -= OnMinigameSetup;
        GameMenu.GameState.OnMinigameCancelled -= OnMinigameCancelled;
        UnsubscribeFromPuzzle();
    }

    private void OnMinigameSetup(string sceneName)
    {
        if (sceneName == PUZZLE_SCENE)
        {
            SubscribeToPuzzle();
            canInteract = false; // Disable interaction while puzzle is active
        }
    }

    private void OnMinigameCancelled()
    {
        if (!isCompleted)
        {
            canInteract = true; // Re-enable interaction if puzzle wasn't completed
            CallInteractionCompletedEvent();
            GameEvents.Instance.DeactivatePlayerInput(false);
        }
    }

    private void SubscribeToPuzzle()
    {
        if (FlamePuzzleManager.Instance != null)
        {
            UnsubscribeFromPuzzle(); // Ensure we don't subscribe twice
            FlamePuzzleManager.Instance.PuzzleCompleted += OnPuzzleCompleted;
        }
    }

    private void UnsubscribeFromPuzzle()
    {
        if (FlamePuzzleManager.Instance != null)
        {
            FlamePuzzleManager.Instance.PuzzleCompleted -= OnPuzzleCompleted;
        }
    }

    /// <summary>
    /// Called after state is initialized from Lua.
    /// Override this to customize behavior when state is initialized.
    /// </summary>
    /// <param name="state">The initialized state value</param>
    protected override void OnStateInitialized(bool state)
    {
        base.OnStateInitialized(state);
        isCompleted = state;
        canInteract = !isCompleted;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Handles interaction with the flame trigger when activated by the player.
    /// Opens the flame puzzle minigame scene.
    /// </summary>
    public override void Interact()
    {
        if (isCompleted) return;
        
        base.Interact();
        GameEvents.Instance.DeactivatePlayerInput(true);
        GameEvents.Instance.PuzzleOpen(PUZZLE_SCENE, m_puzzleIDs);
    }
    #endregion

    #region Private Methods
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
    private void OnPuzzleCompleted(bool isLastPuzzle)
    {
        if (!isLastPuzzle) return; // Only unlock gate when all puzzles are completed
        
        Debug.Log("All puzzles completed, unlocking gate...");
        UnlockAndNotify();
        UpdateVisuals(true);
        isCompleted = true;
        GameEvents.Instance.DeactivatePlayerInput(false);
        UnsubscribeFromPuzzle();
    }
    #endregion
}
