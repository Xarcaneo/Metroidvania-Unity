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
    /// Manages the state of the spell hotbar
    /// </summary>
    public HotbarStateManager HotbarState { get; private set; }
    
    // SpellModifierActive is now handled by the GamepadInputHandler
    
    /// <summary>
    /// Gets the current hotbar number (for backward compatibility)
    /// </summary>
    public int UseSpellHotbarNumber => HotbarState.UseSpellHotbarNumber;

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
    // Add a cooldown timer to prevent rapid switching between input devices
    private float inputDeviceSwitchCooldown = 0.5f; // Half-second cooldown
    private float lastInputDeviceSwitch = 0f;
    
    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        SubscribeToEvents();
        
        // Initialize the hotbar state manager
        HotbarState = new HotbarStateManager();
        
        // Initialize input handlers
        keyboardInputHandler = new KeyboardInputHandler(this);
        gamepadInputHandler = new GamepadInputHandler(this);
        
        // Set initial input handler based on current device
        currentInputHandler = CurrentInputDevice == InputDevice.Keyboard ? keyboardInputHandler : gamepadInputHandler;
        
        // Initialize the cooldown timer
        lastInputDeviceSwitch = Time.time;
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
        // Don't switch devices if we're within the cooldown period
        if (Time.time - lastInputDeviceSwitch < inputDeviceSwitchCooldown)
            return;
            
        // Track if we need to switch devices
        bool shouldSwitchToGamepad = false;
        bool shouldSwitchToKeyboard = false;
        
        // Check if gamepad is connected and has significant input
        if (Gamepad.current != null)
        {        
            // Check for any gamepad button or stick input with significant values
            // This helps filter out noise and phantom inputs
            if (Gamepad.current.wasUpdatedThisFrame)
            {
                // Look for significant stick movement or button presses
                if (Vector2.SqrMagnitude(Gamepad.current.leftStick.ReadValue()) > 0.25f ||
                    Vector2.SqrMagnitude(Gamepad.current.rightStick.ReadValue()) > 0.25f ||
                    Gamepad.current.buttonSouth.isPressed || Gamepad.current.buttonNorth.isPressed ||
                    Gamepad.current.buttonEast.isPressed || Gamepad.current.buttonWest.isPressed ||
                    Gamepad.current.leftTrigger.ReadValue() > 0.5f || Gamepad.current.rightTrigger.ReadValue() > 0.5f ||
                    Gamepad.current.leftShoulder.isPressed || Gamepad.current.rightShoulder.isPressed)
                {
                    shouldSwitchToGamepad = true;
                }
            }
        }
        
        // Check for significant keyboard/mouse input
        if (Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame)
        {
            // Look for actual key presses, not just noise
            if (Keyboard.current.anyKey.isPressed)
            {
                shouldSwitchToKeyboard = true;
            }
        }
        else if (Mouse.current != null && Mouse.current.wasUpdatedThisFrame)
        {            
            // Check for significant mouse movement or clicks
            if (Mouse.current.delta.ReadValue().sqrMagnitude > 1.0f ||
                Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed)
            {
                shouldSwitchToKeyboard = true;
            }
        }
        
        // Prioritize gamepad input if both are detected in the same frame
        if (shouldSwitchToGamepad && CurrentInputDevice != InputDevice.Gamepad)
        {
            CurrentInputDevice = InputDevice.Gamepad;
            currentInputHandler = gamepadInputHandler;
            lastInputDeviceSwitch = Time.time;
            Debug.Log("Switched to Gamepad input");
        }
        else if (shouldSwitchToKeyboard && !shouldSwitchToGamepad && CurrentInputDevice != InputDevice.Keyboard)
        {
            CurrentInputDevice = InputDevice.Keyboard;
            currentInputHandler = keyboardInputHandler;
            lastInputDeviceSwitch = Time.time;
            Debug.Log("Switched to Keyboard input");
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
    /// Resets both SpellCastInput and hotbar state to prevent unintended spell casting
    /// </remarks>
    public void UseSpellCastInput()
    {
        SpellCastInput = false;
        
        // Deactivate the current spell slot but remember it
        HotbarState.DeactivateSlot();
        
        Debug.Log($"Spell cast consumed: Hotbar {HotbarState.CurrentSlot}, Active: {false}");
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