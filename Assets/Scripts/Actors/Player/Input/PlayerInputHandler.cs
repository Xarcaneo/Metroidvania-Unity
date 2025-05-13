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
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    #region Components
    private PlayerInput playerInput;
    #endregion

    #region Input Properties
    /// <summary>
    /// Raw movement input vector from input device
    /// </summary>
    public Vector2 RawMovementInput { get; private set; }

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

    // Flags to track if inputs have been processed to prevent double-triggering with analog triggers
    private bool actionInputProcessed = false;
    private bool attackInputProcessed = false;
    private bool blockInputProcessed = false;
    private bool hotbarActionInputProcessed = false;
    private bool itemSwitchLeftInputProcessed = false;
    private bool itemSwitchRightInputProcessed = false;
    private bool jumpInputProcessed = false;
    private bool spellCastInputProcessed = false;
    private bool interactInputProcessed = false;
    private bool playerMenuInputProcessed = false;

    /// <summary>
    /// Normalized X-axis input (-1, 0, 1)
    /// </summary>
    public int NormInputX { get; private set; }

    /// <summary>
    /// Normalized Y-axis input (-1, 0, 1)
    /// </summary>
    public int NormInputY { get; private set; }

    /// <summary>
    /// Jump button pressed state
    /// </summary>
    public bool JumpInput { get; private set; }

    /// <summary>
    /// Jump button released state
    /// </summary>
    public bool JumpInputStop { get; private set; }

    /// <summary>
    /// Action button (dash) pressed state
    /// </summary>
    public bool ActionInput { get; private set; }

    /// <summary>
    /// Attack button pressed state
    /// </summary>
    public bool AttackInput { get; private set; }

    /// <summary>
    /// Block button pressed state
    /// </summary>
    public bool BlockInput { get; private set; }

    /// <summary>
    /// Hotbar action button pressed state
    /// </summary>
    public bool HotbarActionInput { get; private set; }

    /// <summary>
    /// Item switch left button pressed state
    /// </summary>
    public bool ItemSwitchLeftInput { get; private set; }

    /// <summary>
    /// Item switch right button pressed state
    /// </summary>
    public bool ItemSwitchRightInput { get; private set; }

    /// <summary>
    /// Hotbar number currently triggered with activation state.
    /// Key: Activation state (true = active, false = inactive)
    /// Value: Hotbar number (0, 1, 2 for keyboard, 3, 4, 5 for gamepad)
    /// </summary>
    public Dictionary<bool, int> UseSpellHotbarDictionary { get; private set; } = new Dictionary<bool, int>() { { false, 0 } };
    
    /// <summary>
    /// Flag to track if the LT modifier is being held for gamepad spell casting
    /// </summary>
    private bool SpellModifierActive => Gamepad.current != null && Gamepad.current.leftTrigger.ReadValue() > GamepadControls.TriggerThreshold;
    
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
    public bool SpellCastInput { get; private set; }
    #endregion

    #region Input Settings
    [SerializeField, Tooltip("Maximum time to hold input before it's cleared")]
    private float inputHoldTime = 0.2f;
    #endregion

    #region Input Timers
    private float jumpInputStartTime;
    private float actionInputInputStartTime;
    #endregion

    #region Unity Callback Functions
    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        SubscribeToEvents();
        
        // Initialize the spell hotbar dictionary
        UseSpellHotbarDictionary = new Dictionary<bool, int>() { { false, 0 } };
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void Update()
    {
        CheckJumpInputHoldTime();
        CheckDashInputHoldTime();
        CheckSpellModifierReleased();
        DetectCurrentInputDevice();
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
                Debug.Log("Switched to Keyboard input");
            }
        }
    }
    
    /// <summary>
    /// Checks if the LT modifier has been released while a spell is active
    /// Only applies to gamepad inputs
    /// </summary>
    private void CheckSpellModifierReleased()
    {
        // Only check for gamepad modifier release if we're using a gamepad
        if (CurrentInputDevice != InputDevice.Gamepad)
            return;
            
        // If we're casting a spell and the LT trigger is released, cancel the spell
        if (SpellCastInput && UseSpellHotbarDictionary.ContainsKey(true) && !SpellModifierActive)
        {
            Debug.Log("Gamepad LT trigger released while spell was active - canceling spell");
            SpellCastInput = false;
            
            // Update the dictionary with inactive state but keep the hotbar number
            int lastSlot = UseSpellHotbarDictionary[true];
            UseSpellHotbarDictionary[false] = lastSlot;
            UseSpellHotbarDictionary.Remove(true);
            
            // Reset the processed flag to allow new inputs
            spellCastInputProcessed = false;
        }
    }
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
        if (!DisableInput)
        {
            if (context.started && !attackInputProcessed)
            {
                AttackInput = true;
                attackInputProcessed = true;
            }
            else if (context.canceled)
            {
                AttackInput = false;
                attackInputProcessed = false;
            }
        }
    }

    /// <summary>
    /// Handles block input events from the Input System
    /// </summary>
    public void OnBlockInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started && !blockInputProcessed)
            {
                BlockInput = true;
                blockInputProcessed = true;
            }
            else if (context.canceled)
            {
                BlockInput = false;
                blockInputProcessed = false;
            }
        }
    }

    /// <summary>
    /// Handles hotbar action input events from the Input System
    /// </summary>
    public void OnHotbarActionInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started && !hotbarActionInputProcessed)
            {
                HotbarActionInput = true;
                hotbarActionInputProcessed = true;
            }
            else if (context.canceled)
            {
                HotbarActionInput = false;
                hotbarActionInputProcessed = false;
            }
        }
    }

    /// <summary>
    /// Handles item switch left input events from the Input System
    /// </summary>
    public void OnItemSwitchLeftInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started && !itemSwitchLeftInputProcessed)
            {
                ItemSwitchLeftInput = true;
                itemSwitchLeftInputProcessed = true;
            }
            else if (context.canceled)
            {
                ItemSwitchLeftInput = false;
                itemSwitchLeftInputProcessed = false;
            }
        }
    }

    /// <summary>
    /// Handles item switch right input events from the Input System
    /// </summary>
    public void OnItemSwitchRightInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started && !itemSwitchRightInputProcessed)
            {
                ItemSwitchRightInput = true;
                itemSwitchRightInputProcessed = true;
            }
            else if (context.canceled)
            {
                ItemSwitchRightInput = false;
                itemSwitchRightInputProcessed = false;
            }
        }
    }

    /// <summary>
    /// Handles movement input events from the Input System
    /// </summary>
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            RawMovementInput = context.ReadValue<Vector2>();
            NormInputX = Mathf.RoundToInt(RawMovementInput.x);

            if (Mathf.Abs(RawMovementInput.y) > 0.5f)
            {
                NormInputY = (int)(RawMovementInput * Vector2.up).normalized.y;
            }
            else
            {
                NormInputY = 0;
            }
        }
    }

    // Removed the OnSpellModifierInput method as we're now directly checking the LT trigger in OnUseSpellInput

    /// <summary>
    /// Handles spell input events from the Input System.
    /// Determines the spell type based on the binding index (e.g., first, second, third).
    /// </summary>
    public void OnUseSpellInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            // Get the index of the triggered binding
            var bindingIndex = context.action.GetBindingIndexForControl(context.control);
            
            // Determine if this is a gamepad input
            bool isGamepadInput = IsGamepadBindingIndex(bindingIndex);
            
            // Map the binding index to the appropriate hotbar slot based on input device
            int actualSlot = MapBindingIndexToHotbarSlot(bindingIndex, isGamepadInput);
            
            Debug.Log($"Binding index {bindingIndex} mapped to slot {actualSlot}");
            
            // Process input based on the current device type
            if (isGamepadInput)
            {
                HandleGamepadSpellInput(context, bindingIndex, actualSlot);
            }
            else
            {
                HandleKeyboardSpellInput(context, bindingIndex, actualSlot);
            }
        }
    }
    
    /// <summary>
    /// Determines if a binding index corresponds to a gamepad input
    /// </summary>
    private bool IsGamepadBindingIndex(int bindingIndex)
    {
        return bindingIndex >= GamepadControls.FirstButtonIndex && 
               bindingIndex <= GamepadControls.ThirdButtonIndex;
    }
    
    /// <summary>
    /// Maps a binding index to the appropriate hotbar slot based on input device
    /// </summary>
    private int MapBindingIndexToHotbarSlot(int bindingIndex, bool isGamepadInput)
    {
        if (isGamepadInput)
        {
            // Map gamepad binding indices to specific slots
            switch (bindingIndex)
            {
                case GamepadControls.FirstButtonIndex: // Index 3
                    return GamepadControls.FirstButtonSlot; // Maps to slot 0
                case GamepadControls.SecondButtonIndex: // Index 4
                    return GamepadControls.SecondButtonSlot; // Maps to slot 0
                case GamepadControls.ThirdButtonIndex: // Index 5
                    return GamepadControls.ThirdButtonSlot; // Maps to slot 1
                default: 
                    return 0;
            }
        }
        else
        {
            // Keyboard bindings map directly
            return bindingIndex;
        }
    }
    
    /// <summary>
    /// Handles keyboard-specific spell input logic
    /// </summary>
    private void HandleKeyboardSpellInput(InputAction.CallbackContext context, int bindingIndex, int hotbarSlot)
    {
        if (context.started && !spellCastInputProcessed)
        {
            SpellCastInput = true;
            spellCastInputProcessed = true;
            
            // Update the dictionary with active state and current hotbar number
            UseSpellHotbarDictionary[true] = hotbarSlot;
            Debug.Log($"Keyboard spell activated: Hotbar {hotbarSlot}, Active: {true}");
        }
        else if (context.canceled)
        {
            // Only reset SpellCastInput if this is the key we're currently using
            if (UseSpellHotbarDictionary.ContainsKey(true) && hotbarSlot == UseSpellHotbarDictionary[true])
            {
                SpellCastInput = false;
                
                // Update the dictionary with inactive state but keep the hotbar number
                UseSpellHotbarDictionary[false] = hotbarSlot;
                if (UseSpellHotbarDictionary.ContainsKey(true))
                {
                    UseSpellHotbarDictionary.Remove(true);
                }
                Debug.Log($"Keyboard spell deactivated: Hotbar {hotbarSlot}, Active: {false}");
            }
            spellCastInputProcessed = false;
            return;
        }

        // Update the hotbar number
        UseSpellHotbarDictionary[false] = hotbarSlot;
        Debug.Log($"Keyboard spell hotbar set to: {hotbarSlot}");
    }
    
    /// <summary>
    /// Handles gamepad-specific spell input logic
    /// </summary>
    private void HandleGamepadSpellInput(InputAction.CallbackContext context, int bindingIndex, int hotbarSlot)
    {
        // Debug the LT trigger value
        float ltValue = Gamepad.current != null ? Gamepad.current.leftTrigger.ReadValue() : 0f;
        Debug.Log($"LT Trigger Value: {ltValue}");
        
        if (context.started && !spellCastInputProcessed)
        {
            // For gamepad inputs, only activate if the modifier is active
            Debug.Log($"Gamepad spell button check - Button: {bindingIndex}, ModifierActive: {SpellModifierActive}, LT Value: {ltValue}");
            
            if (!SpellModifierActive)
            {
                Debug.Log($"Gamepad spell button pressed without modifier: {bindingIndex}");
                return; // Don't process this input without the modifier
            }
            
            SpellCastInput = true;
            spellCastInputProcessed = true;
            
            // Update the dictionary with active state and current hotbar number
            UseSpellHotbarDictionary[true] = hotbarSlot;
            Debug.Log($"Gamepad spell activated: Hotbar {hotbarSlot}, Active: {true}");
        }
        else if (context.canceled)
        {
            // Only reset SpellCastInput if this is the button we're currently using
            if (UseSpellHotbarDictionary.ContainsKey(true) && hotbarSlot == UseSpellHotbarDictionary[true])
            {
                SpellCastInput = false;
                
                // Update the dictionary with inactive state but keep the hotbar number
                UseSpellHotbarDictionary[false] = hotbarSlot;
                if (UseSpellHotbarDictionary.ContainsKey(true))
                {
                    UseSpellHotbarDictionary.Remove(true);
                }
                Debug.Log($"Gamepad spell deactivated: Hotbar {hotbarSlot}, Active: {false}");
            }
            spellCastInputProcessed = false;
            return;
        }

        // Only update the hotbar number if the modifier is active
        if (SpellModifierActive)
        {
            UseSpellHotbarDictionary[false] = hotbarSlot;
            Debug.Log($"Gamepad spell hotbar set to: {hotbarSlot}");
        }
        else
        {
            Debug.Log($"Ignoring hotbar update for gamepad button {bindingIndex} without modifier");            
        }
    }

    /// <summary>
    /// Handles jump input events from the Input System
    /// </summary>
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started && !jumpInputProcessed)
            {
                JumpInput = true;
                JumpInputStop = false;
                jumpInputStartTime = Time.time;
                jumpInputProcessed = true;
            }
            else if (context.canceled)
            {
                JumpInputStop = true;
                jumpInputProcessed = false;
            }
        }
    }

    /// <summary>
    /// Handles dash input events from the Input System
    /// </summary>
    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started && !actionInputProcessed)
            {
                ActionInput = true;
                actionInputProcessed = true;
                actionInputInputStartTime = Time.time;
            }
            else if (context.canceled)
            {
                ActionInput = false;
                actionInputProcessed = false;
            }
        }
    }

    /// <summary>
    /// Handles interact input events from the Input System
    /// </summary>
    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (!DisableInput)
        {
            if (context.started && !interactInputProcessed)
            {
                GameEvents.Instance.InteractTrigger(true);
                interactInputProcessed = true;
            }
            else if (context.canceled)
            {
                GameEvents.Instance.InteractTrigger(false);
                interactInputProcessed = false;
            }
        }
    }

    /// <summary>
    /// Handles interact input events from the Input System
    /// </summary>
    public void OnPlayerMenuInput(InputAction.CallbackContext context)
    {
        if (!isDialogueActive)
        {
            if (context.started && !playerMenuInputProcessed)
            {
                GameEvents.Instance.PlayerMenuOpen();
                playerMenuInputProcessed = true;
            }
            else if (context.canceled)
            {
                playerMenuInputProcessed = false;
            }
        }
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