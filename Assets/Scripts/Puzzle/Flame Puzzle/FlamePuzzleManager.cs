using PixelCrushers.DialogueSystem;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityCore.GameManager;
using Menu;
using System.Collections;

/// <summary>
/// Manages the flame puzzle system across the game, handling puzzle instantiation and completion.
/// Implements the Singleton pattern to ensure only one instance exists.
/// </summary>
public class FlamePuzzleManager : MonoBehaviour
{
    private static FlamePuzzleManager _instance;
    public static FlamePuzzleManager Instance => _instance;

    [Header("Puzzle Settings")]
    [Tooltip("Array of puzzle prefabs that can be instantiated")]
    [SerializeField] private GameObject[] puzzlePrefabs;

    /// <summary>
    /// Event triggered when a single puzzle is completed.
    /// </summary>
    public event Action<bool> PuzzleCompleted;

    private GameObject currentPuzzleInstance;
    private int[] remainingPuzzleIds;
    private int currentPuzzleIndex = -1;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        GameMenu.GameState.OnMinigameSetup += OnMinigameSetup;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            GameMenu.GameState.OnMinigameSetup -= OnMinigameSetup;
            CleanupCurrentPuzzle();
        }
    }

    private void CleanupCurrentPuzzle()
    {
        if (currentPuzzleInstance != null)
        {
            Destroy(currentPuzzleInstance);
            currentPuzzleInstance = null;
        }
    }

    private void OnMinigameSetup(string sceneName)
    {
        if (sceneName == "Flame Puzzle")
        {
            remainingPuzzleIds = GameMenu.GameState.GetCurrentPuzzleIds();
            currentPuzzleIndex = -1;
            isTransitioning = false;
            StartCoroutine(LoadNextPuzzleRoutine());
        }
    }

    private IEnumerator LoadNextPuzzleRoutine()
    {
        if (isTransitioning) yield break;

        isTransitioning = true;
        CleanupCurrentPuzzle();

        // Wait a frame to ensure cleanup is complete
        yield return null;
        
        currentPuzzleIndex++;
        if (remainingPuzzleIds != null && currentPuzzleIndex < remainingPuzzleIds.Length)
        {
            int puzzleId = remainingPuzzleIds[currentPuzzleIndex];
            if (puzzleId >= 0 && puzzleId < puzzlePrefabs.Length)
            {
                currentPuzzleInstance = Instantiate(puzzlePrefabs[puzzleId]);
                Debug.Log($"Loading puzzle {currentPuzzleIndex + 1} of {remainingPuzzleIds.Length} (ID: {puzzleId})");
            }
            else
            {
                Debug.LogError($"Invalid puzzle ID: {puzzleId}");
                FinishAllPuzzles();
            }
        }
        else
        {
            // All puzzles completed
            Debug.Log("All puzzles completed!");
            FinishAllPuzzles();
            yield break;
        }

        isTransitioning = false;
    }

    private void FinishAllPuzzles()
    {
        bool isLastPuzzle = true;
        PuzzleCompleted?.Invoke(isLastPuzzle);
        CleanupCurrentPuzzle();
        GameMenu.GameState.CloseMinigame();
    }

    /// <summary>
    /// Called when a puzzle is completed. Updates the game state and triggers relevant events.
    /// </summary>
    public void OnPuzzleCompleted()
    {
        if (isTransitioning) return;

        Debug.Log($"Puzzle {currentPuzzleIndex + 1} of {remainingPuzzleIds.Length} completed!");
        bool isLastPuzzle = currentPuzzleIndex + 1 >= remainingPuzzleIds.Length;
        PuzzleCompleted?.Invoke(isLastPuzzle);

        if (!isLastPuzzle)
        {
            // Use coroutine for better transition control
            StartCoroutine(LoadNextPuzzleRoutine());
        }
        else
        {
            Invoke(nameof(FinishAllPuzzles), 0.5f);
        }
    }
}
