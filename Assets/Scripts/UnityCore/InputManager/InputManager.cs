using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages player input and handles input-related events for menus and navigation.
/// Implements a singleton pattern to ensure only one instance is active at any time.
/// </summary>
public class InputManager : MonoBehaviour
{
    private static InputManager _instance;

    /// <summary>
    /// Singleton instance of the InputManager.
    /// </summary>
    public static InputManager Instance => _instance;

    /// <summary>
    /// The PlayerInput component handling menu inputs.
    /// </summary>
    [SerializeField] private PlayerInput menuInput;

    /// <summary>
    /// Indicates whether input is currently active.
    /// </summary>
    public bool isInputActive = true;

    /// <summary>
    /// Event triggered when the menu return input is activated.
    /// </summary>
    public event Action OnMenuReturn;

    /// <summary>
    /// Event triggered when the pause input is activated.
    /// </summary>
    public event Action OnPause;

    public event Action OnMenuPreviousTab;
    public event Action OnMenuNextTab;
    public event Action OnMenuDelete;
    public event Action OnCategoryUp;
    public event Action OnCategoryDown;
    public event Action<int> OnAssignSpellToHotbar;

    /// <summary>
    /// Ensures only one instance of InputManager exists.
    /// </summary>
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        if (menuInput == null)
        {
            Debug.LogError("PlayerInput component is not assigned.");
        }
    }

    /// <summary>
    /// Subscribes to events when the script starts.
    /// </summary>
    private void Start()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onDialogueTrigger += OnDialogueTrigger;
        }
        else
        {
            Debug.LogWarning("GameEvents instance is not available.");
        }
    }

    /// <summary>
    /// Unsubscribes from events when the script is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }

        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onDialogueTrigger -= OnDialogueTrigger;
        }
    }

    /// <summary>
    /// Updates the input state each frame.
    /// </summary>
    private void Update()
    {
        MenuInputUpdate();
    }

    /// <summary>
    /// Updates the menu input state based on player input.
    /// </summary>
    private void MenuInputUpdate()
    {
        if (!isInputActive || menuInput == null) return;

        if (menuInput.actions["Return"].triggered) OnMenuReturn?.Invoke();
        if (menuInput.actions["Pause"].triggered) OnPause?.Invoke();
        if (menuInput.actions["PreviousTab"].triggered) OnMenuPreviousTab?.Invoke();
        if (menuInput.actions["NextTab"].triggered) OnMenuNextTab?.Invoke();
        if (menuInput.actions["Delete"].triggered) OnMenuDelete?.Invoke();
        if (menuInput.actions["CategoryUp"].triggered) OnCategoryUp?.Invoke();
        if (menuInput.actions["CategoryDown"].triggered) OnCategoryDown?.Invoke();

        // Handle hotbar assignment
        if (menuInput.actions["AssignSpellToHotbar"].triggered)
        {
            // Check for keyboard keys or gamepad buttons
            if (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame || 
                Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame) // Y button
            {
                OnAssignSpellToHotbar?.Invoke(0);
            }
            else if (Keyboard.current != null && Keyboard.current.digit2Key.wasPressedThisFrame || 
                     Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame) // X button
            {
                OnAssignSpellToHotbar?.Invoke(1);
            }
            else if (Keyboard.current != null && Keyboard.current.digit3Key.wasPressedThisFrame || 
                     Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) // A button
            {
                OnAssignSpellToHotbar?.Invoke(2);
            }
        }
    }

    /// <summary>
    /// Toggles input activity when a dialogue is triggered.
    /// </summary>
    /// <param name="dialogueState">Indicates whether a dialogue is active.</param>
    private void OnDialogueTrigger(bool dialogueState)
    {
        isInputActive = !dialogueState;
    }

    /// <summary>
    /// Gets the navigation input value.
    /// </summary>
    /// <returns>A Vector2 representing the navigation input value.</returns>
    public Vector2 GetNavigateValue()
    {
        if (menuInput == null) return Vector2.zero;

        return menuInput.actions["Navigate"].ReadValue<Vector2>();
    }
}
