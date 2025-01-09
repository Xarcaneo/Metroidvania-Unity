using PixelCrushers.DialogueSystem;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the flame puzzle system across the game, handling puzzle instantiation and completion.
/// Implements the Singleton pattern to ensure only one instance exists.
/// </summary>
public class FlamePuzzleManager : MonoBehaviour
{
    private static FlamePuzzleManager _instance;

    /// <summary>
    /// Gets the singleton instance of the FlamePuzzleManager.
    /// </summary>
    public static FlamePuzzleManager Instance => _instance;

    [Header("Puzzle Settings")]
    [Tooltip("Array of puzzle prefabs that can be instantiated")]
    [SerializeField] private GameObject[] puzzlePrefabs;

    private string m_triggerID;

    /// <summary>
    /// Event triggered when any flame puzzle is completed.
    /// </summary>
    public event Action PuzzleCompleted;

    /// <summary>
    /// Initializes the singleton instance on Awake.
    /// </summary>
    private void Awake()
    {
        InitializeSingleton();
    }

    /// <summary>
    /// Sets up the singleton pattern, ensuring only one instance exists.
    /// </summary>
    private void InitializeSingleton()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    /// <summary>
    /// Instantiates a puzzle prefab in the specified scene.
    /// </summary>
    /// <param name="triggerID">ID of the trigger that activated this puzzle</param>
    /// <param name="puzzleID">Index of the puzzle prefab to instantiate</param>
    /// <param name="sceneName">Name of the scene to instantiate the puzzle in</param>
    /// <returns>The instantiated puzzle GameObject, or null if instantiation fails</returns>
    public GameObject InstantiateObject(string triggerID, int puzzleID, string sceneName)
    {
        if (!ValidatePuzzleParameters(puzzleID, sceneName, out Scene targetScene))
        {
            return null;
        }

        m_triggerID = triggerID;
        GameObject prefabToInstantiate = puzzlePrefabs[puzzleID];
        GameObject instance = Instantiate(prefabToInstantiate, Vector3.zero, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(instance, targetScene);
        SceneManager.SetActiveScene(targetScene);
        
        return instance;
    }

    /// <summary>
    /// Called when a puzzle is completed. Updates the game state and triggers relevant events.
    /// Sets the trigger variable in the dialogue system and notifies the game event system.
    /// </summary>
    public void OnPuzzleCompleted()
    {
        PuzzleCompleted?.Invoke();
        DialogueLua.SetVariable("Trigger." + m_triggerID, true);
        GameEvents.Instance.TriggerStateChanged(m_triggerID);
    }

    /// <summary>
    /// Validates the puzzle parameters and ensures the target scene exists.
    /// </summary>
    /// <param name="puzzleID">ID of the puzzle to validate</param>
    /// <param name="sceneName">Name of the target scene</param>
    /// <param name="targetScene">Output parameter containing the target scene if valid</param>
    /// <returns>True if parameters are valid, false otherwise</returns>
    private bool ValidatePuzzleParameters(int puzzleID, string sceneName, out Scene targetScene)
    {
        targetScene = SceneManager.GetSceneByName(sceneName);

        if (puzzleID < 0 || puzzleID >= puzzlePrefabs.Length)
        {
            Debug.LogError($"Invalid puzzle ID: {puzzleID}");
            return false;
        }

        if (!targetScene.IsValid())
        {
            Debug.LogError($"Scene {sceneName} not found.");
            return false;
        }

        return true;
    }
}
