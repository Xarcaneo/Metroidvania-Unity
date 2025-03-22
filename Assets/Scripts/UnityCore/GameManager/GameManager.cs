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
    #region Constants
    private const bool DEBUG_KEYBINDINGS = false;
    #endregion

    #region Serialized Fields
    [Header("Input Settings")]
    [Tooltip("Reference to the Input Action Asset containing all input bindings")]
    [SerializeField] private InputActionAsset actions;

    [Header("Game Systems")]
    [Tooltip("Reference to the SpellsCatalogue ScriptableObject")]
    [SerializeField] private SpellsCatalogue spellsCatalogue;
    #endregion

    #region Public Properties
    /// <summary>
    /// The currently active save slot number.
    /// Used for managing multiple save files.
    /// </summary>
    public int currentSaveSlot;
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

        // Initialize SpellsCatalogue
        if (spellsCatalogue != null)
        {
            SpellsCatalogue.Initialize(spellsCatalogue);
        }
        else
        {
            Debug.LogError("SpellsCatalogue reference is missing in GameManager!");
        }

        // Lock and hide cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

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

                    #if UNITY_EDITOR && DEBUG_KEYBINDINGS
                    Debug.Log($"[GameManager] Updated binding for {action.name}: {keyName}");
                    #endif
                }
            }
        }
    }

    /// <summary>
    /// Retrieves the current keybinding display string for a specific action.
    /// For composite actions (like Movement), you can specify a part name (e.g., "Movement.down")
    /// For indexed actions (like UseSpell), you can specify the index (e.g., "UseSpell.1")
    /// </summary>
    /// <param name="actionName">Name of the input action to look up, optionally with part name or index</param>
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

        // Split action name to handle composite parts (e.g., "Movement.down") or indices (e.g., "UseSpell.1")
        string[] parts = actionName.Split('.');
        string baseActionName = parts[0];
        string compositePart = parts.Length > 1 ? parts[1].ToLower() : string.Empty;

        foreach (InputActionMap actionMap in actions.actionMaps)
        {
            InputAction action = actionMap.FindAction(baseActionName);

            if (action != null)
            {
                // For composite bindings (like Movement)
                if (!string.IsNullOrEmpty(compositePart))
                {
                    // For numeric indices (like UseSpell.1)
                    if (int.TryParse(compositePart, out int index))
                    {
                        // Find the binding for this index (1-based to 0-based)
                        int targetIndex = index - 1;
                        int currentIndex = 0;
                        
                        foreach (InputBinding binding in action.bindings)
                        {
                            if (!binding.isPartOfComposite)
                            {
                                if (currentIndex == targetIndex)
                                {
                                    return binding.ToDisplayString(
                                        InputBinding.DisplayStringOptions.DontIncludeInteractions);
                                }
                                currentIndex++;
                            }
                        }
                    }
                    // For named composite parts (like Movement.down)
                    else
                    {
                        foreach (InputBinding binding in action.bindings)
                        {
                            if (binding.isPartOfComposite && binding.name.ToLower() == compositePart)
                            {
                                return binding.ToDisplayString(
                                    InputBinding.DisplayStringOptions.DontIncludeInteractions);
                            }
                        }
                    }
                }
                // For regular bindings
                else
                {
                    foreach (InputBinding binding in action.bindings)
                    {
                        if (!binding.isPartOfComposite)
                        {
                            return binding.ToDisplayString(
                                InputBinding.DisplayStringOptions.DontIncludeInteractions);
                        }
                    }
                }
            }
        }

        Debug.LogWarning($"No binding found for action: {actionName}");
        return string.Empty;
    }

    /// <summary>
    /// Forces the cursor to be hidden and locked.
    /// Call this if you need to ensure the cursor is disabled.
    /// </summary>
    public void DisableCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Shows and unlocks the cursor.
    /// Only use this for menus or UI that absolutely requires mouse input.
    /// </summary>
    public void EnableCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    #endregion
}
