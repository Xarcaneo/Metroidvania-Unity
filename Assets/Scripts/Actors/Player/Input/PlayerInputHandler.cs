using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// Struct that defines keyboard control mappings
/// </summary>
public struct KeyboardControls
{
    // Keyboard hotbar slots map directly to indices 0, 1, 2
    public const int FirstSpellSlot = 0;
    public const int SecondSpellSlot = 1;
    public const int ThirdSpellSlot = 2;
}

/// <summary>
/// Struct that defines gamepad control mappings
/// </summary>
public struct GamepadControls
{
    // Gamepad binding indices in the input system
    public const int FirstButtonIndex = 3;
    public const int SecondButtonIndex = 4;
    public const int ThirdButtonIndex = 5;
    
    // Mapping from gamepad binding indices to hotbar slots
    public const int FirstButtonSlot = 0;
    public const int SecondButtonSlot = 0; // Changed to 0 as per user requirement
    public const int ThirdButtonSlot = 1; // Changed to 1 as per user requirement
    
    // Threshold for trigger activation
    public const float TriggerThreshold = 0.5f;
}

/// <summary>
/// Handles all player input using the new Input System.
/// Processes raw input data and provides normalized values for movement and actions.
/// Delegates device-specific input handling to specialized input handlers.
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    #region Components
    private PlayerInput playerInput;
    private IInputHandler keyboardInputHandler;
    private IInputHandler gamepadInputHandler;
    private IInputHandler currentInputHandler;
    #endregion

    #region Input Properties
    /// <summary>
    /// Raw movement input vector from input device
    /// </summary>
    public Vector2 RawMovementInput { get; set; }

    /// <summary>
    /// Flag to disable all input processing
    /// </summary>
    public bool DisableInput = false;

    /// <summary>
    /// Indicates which input device is currently being used
    /// </summary>
    public enum InputDevice
    {
        Keyboard,
        Gamepad
    }

    /// <summary>
    /// The current input device being used
    /// </summary>
    public InputDevice CurrentInputDevice { get; private set; } = InputDevice.Keyboard;

    private bool isDialogueActive = false;
    
    /// <summary>
    /// Public accessor for isDialogueActive for input handlers
    /// </summary>
    public bool IsDialogueActive => isDialogueActive;

    // Input flags are now managed by the individual input handlers

    /// <summary>
    /// Normalized X-axis input (-1, 0, 1)
    /// </summary>
    public int NormInputX { get; set; }

    /// <summary>
    /// Normalized Y-axis input (-1, 0, 1)
    /// </summary>
    public int NormInputY { get; set; }

    /// <summary>
    /// Jump button pressed state
    /// </summary>
    public bool JumpInput { get; set; }

    /// <summary>
    /// Jump button released state
    /// </summary>
    public bool JumpInputStop { get; set; }

    /// <summary>
    /// Action button (dash) pressed state
    /// </summary>
    public bool ActionInput { get; set; }

    /// <summary>
    /// Attack button pressed state
    /// </summary>
    public bool AttackInput { get; set; }

    /// <summary>
    /// Block button pressed state
    /// </summary>
    public bool BlockInput { get; set; }

    /// <summary>
    /// Hotbar action button pressed state
    /// </summary>
    public bool HotbarActionInput { get; set; }

    /// <summary>
    /// Item switch left button pressed state
    /// </summary>
    public bool ItemSwitchLeftInput { get; set; }

    /// <summary>
    /// Item switch right button pressed state
    /// </summary>
    public bool ItemSwitchRightInput { get; set; }

    /// <summary>
    /// Hotbar number currently triggered with activation state.
    /// Key: Activation state (true = active, false = inactive)
    /// Value: Hotbar number (0, 1, 2 for keyboard, 3, 4, 5 for gamepad)
    /// </summary>
    public Dictionary<bool, int> UseSpellHotbarDictionary { get; set; } = new Dictionary<bool, int>() { { false, 0 } };
    
    // SpellModifierActive is now handled by the GamepadInputHandler
    
    /// <summary>
    /// Gets the current hotbar number (for backward compatibility)
    /// </summary>
    public int UseSpellHotbarNumber
    {
        get
        {
            int result = 0;
            
            // If there's an active spell, return that hotbar number
            if (UseSpellHotbarDictionary.ContainsKey(true))
            {
                int slot = UseSpellHotbarDictionary[true];
                // Convert gamepad slots (3,4,5) to keyboard slots (0,1,2) for backward compatibility
                result = slot;
                Debug.Log($"UseSpellHotbarNumber returning active slot: {result}");
                return result;
            }
            // Otherwise return the last selected hotbar number
            if (UseSpellHotbarDictionary.ContainsKey(false))
            {
                int slot = UseSpellHotbarDictionary[false];
                // Convert gamepad slots (3,4,5) to keyboard slots (0,1,2) for backward compatibility
                result = slot;
                Debug.Log($"UseSpellHotbarNumber returning inactive slot: {result}");
                return result;
            }
            
            Debug.Log($"UseSpellHotbarNumber returning default: {result}");
            return result;
        }
    }

    /// <summary>
    /// Flag indicating if spell cast button is pressed
    /// </summary>
    /// <remarks>
    /// Set when any spell input is triggered. Used in conjunction with UseSpellType
    /// to determine which spell to cast.
    /// </remarks>
    public bool SpellCastInput { get; set; }
    #endregion

    #region Input Settings
    [SerializeField, Tooltip("Maximum time to hold input before it's cleared")]
    private float inputHoldTime = 0.2f;
    #endregion

    #region Input Timers
    private float jumpInputStartTime;
    private float actionInputInputStartTime;
    
    // Methods to allow input handlers to set these values
    public void SetJumpInputStartTime(float time) => jumpInputStartTime = time;
    public void SetActionInputStartTime(float time) => actionInputInputStartTime = time;
    #endregion

    #region Unity Callback Functions
    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        SubscribeToEvents();
        
        // Initialize the spell hotbar dictionary
        UseSpellHotbarDictionary = new Dictionary<bool, int>() { { false, 0 } };
        
        // Initialize input handlers
        keyboardInputHandler = new KeyboardInputHandler(this);
        gamepadInputHandler = new GamepadInputHandler(this);
        
        // Set initial input handler based on current device
        currentInputHandler = CurrentInputDevice == InputDevice.Keyboard ? keyboardInputHandler : gamepadInputHandler;
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void Update()
    {
        CheckJumpInputHoldTime();
        CheckDashInputHoldTime();
        DetectCurrentInputDevice();
        
        // Update the current input handler
        currentInputHandler?.Update();
    }
    
    /// <summary>
    /// Detects which input device is currently being used based on the last input
    /// </summary>
    private void DetectCurrentInputDevice()
    {
        // Check if gamepad is connected and has recent input
        if (Gamepad.current != null)
        {        
            // Check for any gamepad button or stick input
            if (Gamepad.current.wasUpdatedThisFrame)
            {
                if (CurrentInputDevice != InputDevice.Gamepad)
                {
                    CurrentInputDevice = InputDevice.Gamepad;
                    currentInputHandler = gamepadInputHandler;
                    Debug.Log("Switched to Gamepad input");
                }
                return;
            }
        }
        
        // Check for keyboard/mouse input
        if (Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame || 
            Mouse.current != null && Mouse.current.wasUpdatedThisFrame)
        {
            if (CurrentInputDevice != InputDevice.Keyboard)
            {
                CurrentInputDevice = InputDevice.Keyboard;
                currentInputHandler = keyboardInputHandler;
                Debug.Log("Switched to Keyboard input");
            }
        }
    }
    
    // CheckSpellModifierReleased is now handled by the GamepadInputHandler
    #endregion

    #region Event Management
    private void SubscribeToEvents()
    {
        try
        {
            GameEvents.Instance.onPauseTrigger += EnableDisablePlayerInput;
            GameEvents.Instance.onDialogueTrigger += EnableDisablePlayerInput;
            GameEvents.Instance.onDeactivatePlayerInput += EnableDisablePlayerInput;
            GameEvents.Instance.onDialogueTrigger += OnDialogueTrigger;
        }
        catch
        {
            Debug.LogWarning("Failed to subscribe to game events. GameEvents instance might not be available.");
        }
    }

    private void UnsubscribeFromEvents()
    {
        try
        {
            GameEvents.Instance.onPauseTrigger -= EnableDisablePlayerInput;
            GameEvents.Instance.onDialogueTrigger -= EnableDisablePlayerInput;
            GameEvents.Instance.onDeactivatePlayerInput -= EnableDisablePlayerInput;
            GameEvents.Instance.onDialogueTrigger -= OnDialogueTrigger;

        }
        catch
        {
            Debug.LogWarning("Failed to unsubscribe from game events. GameEvents instance might not be available.");
        }
    }
    #endregion

    #region Input System Callbacks
    /// <summary>
    /// Handles attack input events from the Input System
    /// </summary>
    public void OnAttackInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, "Attack");
    }

    /// <summary>
    /// Handles block input events from the Input System
    /// </summary>
    public void OnBlockInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, "Block");
    }

    /// <summary>
    /// Handles hotbar action input events from the Input System
    /// </summary>
    public void OnHotbarActionInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, "HotbarAction");
    }

    /// <summary>
    /// Handles item switch left input events from the Input System
    /// </summary>
    public void OnItemSwitchLeftInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, "ItemSwitchLeft");
    }

    /// <summary>
    /// Handles item switch right input events from the Input System
    /// </summary>
    public void OnItemSwitchRightInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, "ItemSwitchRight");
    }

    /// <summary>
    /// Handles movement input events from the Input System
    /// </summary>
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, "Move");
    }

    // Removed the OnSpellModifierInput method as we're now directly checking the LT trigger in OnUseSpellInput

    /// <summary>
    /// Handles spell input events from the Input System.
    /// Delegates to the appropriate input handler based on current device.
    /// </summary>
    public void OnUseSpellInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, "UseSpell");
    }
    
    // IsGamepadBindingIndex, MapBindingIndexToHotbarSlot, HandleKeyboardSpellInput, and HandleGamepadSpellInput
    // have been moved to their respective input handler classes

    /// <summary>
    /// Handles jump input events from the Input System
    /// </summary>
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, "Jump");
    }

    /// <summary>
    /// Handles dash input events from the Input System
    /// </summary>
    public void OnDashInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, "Dash");
    }

    /// <summary>
    /// Handles interact input events from the Input System
    /// </summary>
    public void OnInteractInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, "Interact");
    }

    /// <summary>
    /// Handles player menu input events from the Input System
    /// </summary>
    public void OnPlayerMenuInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, "PlayerMenu");
    }
    #endregion

    #region Input Consumption Methods
    /// <summary>
    /// Consumes the jump input
    /// </summary>
    public void UseJumpInput()
    {
        JumpInput = false;
        // Don't reset jumpInputProcessed here as it should be reset on button release
    }

    /// <summary>
    /// Consumes the dash input
    /// </summary>
    public void UseDashInput()
    {
        ActionInput = false;
        // Don't reset actionInputProcessed here as it should be reset on button release
    }

    /// <summary>
    /// Consumes the attack input
    /// </summary>
    public void UseAttackInput()
    {
        AttackInput = false;
        // Don't reset attackInputProcessed here as it should be reset on button release
    }

    /// <summary>
    /// Consumes the block input
    /// </summary>
    public void UseBlockInput()
    {
        BlockInput = false;
        // Don't reset blockInputProcessed here as it should be reset on button release
    }

    /// <summary>
    /// Consumes the hotbar action input
    /// </summary>
    public void UseHotbarActionInput()
    {
        HotbarActionInput = false;
        // Don't reset hotbarActionInputProcessed here as it should be reset on button release
    }

    /// <summary>
    /// Consumes the item switch left input
    /// </summary>
    public void UseItemSwitchLeftInput()
    {
        ItemSwitchLeftInput = false;
        // Don't reset itemSwitchLeftInputProcessed here as it should be reset on button release
    }

    /// <summary>
    /// Consumes the item switch right input
    /// </summary>
    public void UseItemSwitchRightInput()
    {
        ItemSwitchRightInput = false;
        // Don't reset itemSwitchRightInputProcessed here as it should be reset on button release
    }

    /// <summary>
    /// Consumes the spell cast input
    /// </summary>
    /// <remarks>
    /// Resets both SpellCastInput and UseSpellType to prevent unintended spell casting
    /// </remarks>
    public void UseSpellCastInput()
    {
        SpellCastInput = false;
        
        // Reset the spell hotbar state to inactive but keep the last used number
        int lastHotbarNumber = UseSpellHotbarDictionary.ContainsKey(false) ? UseSpellHotbarDictionary[false] : 0;
        UseSpellHotbarDictionary.Clear();
        UseSpellHotbarDictionary[false] = lastHotbarNumber;
        
        Debug.Log($"Spell cast consumed: Hotbar {lastHotbarNumber}, Active: {false}");
        // Don't reset spellCastInputProcessed here as it should be reset on button release
    }

    #endregion

    #region Input Check Methods
    /// <summary>
    /// Checks if jump input has been held longer than the maximum hold time
    /// </summary>
    private void CheckJumpInputHoldTime()
    {
        if (Time.time >= jumpInputStartTime + inputHoldTime)
        {
            JumpInput = false;
        }
    }

    /// <summary>
    /// Checks if dash input has been held longer than the maximum hold time
    /// </summary>
    private void CheckDashInputHoldTime()
    {
        if (Time.time >= actionInputInputStartTime + inputHoldTime)
        {
            ActionInput = false;
        }
    }
    #endregion

    #region Input State Management
    /// <summary>
    /// Enables or disables all player input
    /// </summary>
    /// <param name="disable">True to disable input, false to enable</param>
    private void EnableDisablePlayerInput(bool disable)
    {
        DisableInput = disable;
        if (disable)
        {
            NormInputX = 0;
            NormInputY = 0;
        }
    }

    private void OnDialogueTrigger(bool isDialogueActive) => this.isDialogueActive = isDialogueActive;
    #endregion
}