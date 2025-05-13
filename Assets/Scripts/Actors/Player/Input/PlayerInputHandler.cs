using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityCore.GameManager;

/// <summary>
/// Struct that defines gamepad control constants
/// </summary>
public struct GamepadControls
{
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
    
    /// <summary>
    /// Updates the current input device and notifies the GameManager
    /// </summary>
    /// <param name="device">The new input device</param>
    private void UpdateCurrentInputDevice(InputDevice device)
    {
        if (CurrentInputDevice != device)
        {
            CurrentInputDevice = device;
            
            // Update the InputDeviceDetector with the new input device type
            var detectorDeviceType = device == InputDevice.Keyboard 
                ? InputDeviceDetector.InputDeviceType.Keyboard 
                : InputDeviceDetector.InputDeviceType.Gamepad;
                
            InputDeviceDetector.UpdateInputDeviceType(detectorDeviceType);
        }
    }

    private bool isDialogueActive = false;
    
    /// <summary>
    /// Public accessor for isDialogueActive for input handlers
    /// </summary>
    public bool IsDialogueActive => isDialogueActive;

    /// <summary>
    /// Flag to track if the spell modifier is active (used to prioritize spell casting)
    /// </summary>
    public bool SpellModifierActive { get; private set; } = false;

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
        // Use the InputDeviceDetector to detect the current input device
        bool deviceSwitched = InputDeviceDetector.DetectCurrentInputDevice(inputDeviceSwitchCooldown);
    
        // If the device changed, update our input handler
        if (deviceSwitched)
        {
            if (InputDeviceDetector.CurrentInputDevice == InputDeviceDetector.InputDeviceType.Gamepad && 
                CurrentInputDevice != InputDevice.Gamepad)
            {
                UpdateCurrentInputDevice(InputDevice.Gamepad);
                currentInputHandler = gamepadInputHandler;
            }
            else if (InputDeviceDetector.CurrentInputDevice == InputDeviceDetector.InputDeviceType.Keyboard && 
                     CurrentInputDevice != InputDevice.Keyboard)
            {
                UpdateCurrentInputDevice(InputDevice.Keyboard);
                currentInputHandler = keyboardInputHandler;
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
        currentInputHandler?.ProcessInput(context, InputActionType.Attack);
    }

    /// <summary>
    /// Handles block input events from the Input System
    /// </summary>
    public void OnBlockInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, InputActionType.Block);
    }

    /// <summary>
    /// Handles hotbar action input events from the Input System
    /// </summary>
    public void OnHotbarActionInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, InputActionType.HotbarAction);
    }

    /// <summary>
    /// Handles item switch left input events from the Input System
    /// </summary>
    public void OnItemSwitchLeftInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, InputActionType.ItemSwitchLeft);
    }

    /// <summary>
    /// Handles item switch right input events from the Input System
    /// </summary>
    public void OnItemSwitchRightInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, InputActionType.ItemSwitchRight);
    }

    /// <summary>
    /// Handles movement input events from the Input System
    /// </summary>
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, InputActionType.Move);
    }

    /// <summary>
    /// Handles input events for spell slot 0 (first slot)
    /// </summary>
    public void OnUseSpellSlot0(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, InputActionType.UseSpellSlot0);
    }
    
    /// <summary>
    /// Handles input events for spell slot 1 (second slot)
    /// </summary>
    public void OnUseSpellSlot1(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, InputActionType.UseSpellSlot1);
    }
    
    /// <summary>
    /// Handles input events for spell slot 2 (third slot)
    /// </summary>
    public void OnUseSpellSlot2(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, InputActionType.UseSpellSlot2);
    }
    
    // IsGamepadBindingIndex, MapBindingIndexToHotbarSlot, HandleKeyboardSpellInput, and HandleGamepadSpellInput
    // have been moved to their respective input handler classes

    /// <summary>
    /// Handles jump input events from the Input System
    /// </summary>
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, InputActionType.Jump);
    }

    /// <summary>
    /// Handles dash input events from the Input System
    /// </summary>
    public void OnDashInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, InputActionType.Dash);
    }

    /// <summary>
    /// Handles interact input events from the Input System
    /// </summary>
    public void OnInteractInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, InputActionType.Interact);
    }

    /// <summary>
    /// Handles player menu input events from the Input System
    /// </summary>
    public void OnPlayerMenuInput(InputAction.CallbackContext context)
    {
        currentInputHandler?.ProcessInput(context, InputActionType.PlayerMenu);
    }
    
    /// <summary>
    /// Handles spell modifier input events from the Input System (e.g., LT button on gamepad)
    /// </summary>
    public void OnSpellModifierInput(InputAction.CallbackContext context)
    {
        // Update the global SpellModifierActive flag based on input state
        if (context.started || context.performed)
        {
            SpellModifierActive = true;
        }
        else if (context.canceled)
        {
            SpellModifierActive = false;
        }
        
        // Also pass to the current input handler for device-specific handling
        currentInputHandler?.ProcessInput(context, InputActionType.SpellModifier);
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