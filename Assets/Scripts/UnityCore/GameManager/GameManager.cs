using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Core game management system that handles global game state and functionality.
/// Implements the singleton pattern to provide centralized access to game systems.
/// Manages input bindings, save slots, and dialogue system integration.
/// </summary>
[DefaultExecutionOrder(-100)] // Ensure GameManager initializes before other managers
public class GameManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("Input Settings")]
    [Tooltip("Reference to the Input Action Asset containing all input bindings")]
    [SerializeField] private InputActionAsset actions;
    #endregion

    #region Public Properties
    /// <summary>
    /// The currently active save slot number.
    /// Used for managing multiple save files.
    /// </summary>
    public int currentSaveSlot;

    /// <summary>
    /// Flag indicating if the player character should be flipped.
    /// Used for controlling character orientation.
    /// </summary>
    public bool shouldFlipPlayer = false;
    #endregion

    #region Singleton Pattern
    private static GameManager _instance;

    /// <summary>
    /// Singleton instance of the GameManager.
    /// Provides global access to game management functionality.
    /// </summary>
    public static GameManager Instance => _instance;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the singleton instance and sets up input system callbacks.
    /// Ensures only one GameManager exists in the scene.
    /// </summary>
    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning($"Found duplicate GameManager in scene. Destroying instance on {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Subscribe to input device changes
        InputSystem.onDeviceChange += OnDeviceChange;
        UpdateBindingToDialogue(); // Initialize bindings
    }

    /// <summary>
    /// Cleans up the singleton instance and unsubscribes from input system events.
    /// </summary>
    private void OnDestroy()
    {
        if (_instance == this)
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
            _instance = null;
        }
    }
    #endregion

    #region Input Management
    /// <summary>
    /// Handles input device change events by updating dialogue system bindings.
    /// </summary>
    /// <param name="device">The input device that changed</param>
    /// <param name="change">The type of change that occurred</param>
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        UpdateBindingToDialogue();
    }

    /// <summary>
    /// Updates the dialogue system with current input bindings.
    /// Maps all input actions to dialogue system variables for UI display.
    /// </summary>
    public void UpdateBindingToDialogue()
    {
        if (actions == null)
        {
            Debug.LogError("Input Action Asset not assigned to GameManager!");
            return;
        }

        foreach (InputActionMap actionMap in actions.actionMaps)
        {
            foreach (InputAction action in actionMap.actions)
            {
                foreach (InputBinding binding in action.bindings)
                {
                    string keyName = binding.ToDisplayString(
                        InputBinding.DisplayStringOptions.DontIncludeInteractions);

                    // Store binding in dialogue system for UI display
                    DialogueLua.SetVariable($"Keybinding.{action.name}", keyName);

                    #if UNITY_EDITOR
                    Debug.Log($"Updated binding for {action.name}: {keyName}");
                    #endif
                }
            }
        }
    }

    /// <summary>
    /// Retrieves the current keybinding display string for a specific action.
    /// </summary>
    /// <param name="actionName">Name of the input action to look up</param>
    /// <returns>Display string for the action's current binding, or empty string if not found</returns>
    public string GetKeybindingForAction(string actionName)
    {
        if (string.IsNullOrEmpty(actionName))
        {
            Debug.LogWarning("GetKeybindingForAction called with null or empty action name");
            return string.Empty;
        }

        if (actions == null)
        {
            Debug.LogError("Input Action Asset not assigned to GameManager!");
            return string.Empty;
        }

        foreach (InputActionMap actionMap in actions.actionMaps)
        {
            InputAction action = actionMap.FindAction(actionName);

            if (action != null)
            {
                foreach (InputBinding binding in action.bindings)
                {
                    return binding.ToDisplayString(
                        InputBinding.DisplayStringOptions.DontIncludeInteractions);
                }
            }
        }

        Debug.LogWarning($"No binding found for action: {actionName}");
        return string.Empty;
    }
    #endregion
}
