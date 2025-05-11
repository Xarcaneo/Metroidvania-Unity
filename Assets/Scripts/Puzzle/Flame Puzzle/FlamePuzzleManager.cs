using PixelCrushers.DialogueSystem;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityCore.GameManager;
using Menu;
using System.Collections;
using UnityEngine.UI;

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

    [Header("Transition Settings")]
    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private float delayBetweenPuzzles = 0.3f;

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
            InitializeFadePanel();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        GameMenu.GameState.OnMinigameSetup += OnMinigameSetup;
    }

    private void InitializeFadePanel()
    {
        if (fadePanel == null)
        {
            // Create fade panel if not assigned
            var go = new GameObject("PuzzleFadePanel");
            go.transform.SetParent(transform);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999; // Ensure it's on top
            
            fadePanel = go.AddComponent<Image>();
            fadePanel.color = Color.clear;
            
            var rect = fadePanel.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
        }
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
            fadePanel.color = Color.clear;
            LoadFirstPuzzle();
        }
    }

    private void LoadFirstPuzzle()
    {
        currentPuzzleIndex++;
        if (remainingPuzzleIds != null && currentPuzzleIndex < remainingPuzzleIds.Length)
        {
            int puzzleId = remainingPuzzleIds[currentPuzzleIndex];
            if (puzzleId >= 0 && puzzleId < puzzlePrefabs.Length)
            {
                currentPuzzleInstance = Instantiate(puzzlePrefabs[puzzleId]);
            }
            else
            {
                Debug.LogError($"Invalid puzzle ID: {puzzleId}");
                FinishAllPuzzles();
            }
        }
    }

    private IEnumerator LoadNextPuzzleRoutine()
    {
        if (isTransitioning) yield break;

        isTransitioning = true;

        // Fade out
        LeanTween.value(fadePanel.gameObject, 0f, 1f, fadeTime)
            .setOnUpdate((float val) => {
                fadePanel.color = new Color(0, 0, 0, val);
            });
        
        yield return new WaitForSeconds(fadeTime);

        CleanupCurrentPuzzle();
        yield return new WaitForSeconds(delayBetweenPuzzles);
        
        currentPuzzleIndex++;
        if (remainingPuzzleIds != null && currentPuzzleIndex < remainingPuzzleIds.Length)
        {
            int puzzleId = remainingPuzzleIds[currentPuzzleIndex];
            if (puzzleId >= 0 && puzzleId < puzzlePrefabs.Length)
            {
                currentPuzzleInstance = Instantiate(puzzlePrefabs[puzzleId]);

                // Fade in
                LeanTween.value(fadePanel.gameObject, 1f, 0f, fadeTime)
                    .setOnUpdate((float val) => {
                        fadePanel.color = new Color(0, 0, 0, val);
                    });
                
                yield return new WaitForSeconds(fadeTime);
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

        bool isLastPuzzle = currentPuzzleIndex + 1 >= remainingPuzzleIds.Length;
        PuzzleCompleted?.Invoke(isLastPuzzle);

        if (!isLastPuzzle)
        {
            // Use coroutine for better transition control
            StartCoroutine(LoadNextPuzzleRoutine());
        }
        else
        {
            FinishAllPuzzles();
        }
    }
}
