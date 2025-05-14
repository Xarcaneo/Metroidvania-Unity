using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// Utility class for detecting and tracking the current input device.
/// This non-MonoBehaviour class can be used by both GameManager and PlayerInputHandler.
/// </summary>
public static class InputDeviceDetector
{
    // Constants - using very small thresholds for quick device switching
    private const float DEFAULT_DEVICE_SWITCH_COOLDOWN = 0.05f; // Reduced from 0.5s to 0.05s
    private const float STICK_THRESHOLD = 0.05f;               // Reduced from 0.25 to 0.05
    private const float MOUSE_MOVEMENT_THRESHOLD = 0.5f;       // Reduced from 1.0 to 0.5
    
    // Last time the input device was switched (using unscaled time to work during pause)
    private static float lastInputDeviceSwitch = 0f;
    
    /// <summary>
    /// Enum representing the current input device being used by the player.
    /// </summary>
    public enum InputDeviceType
    {
        Keyboard,
        Gamepad
    }
    
    /// <summary>
    /// Event triggered when the input device changes.
    /// </summary>
    public static event Action<InputDeviceType> OnInputDeviceChanged;
    
    /// <summary>
    /// The current input device being used by the player.
    /// </summary>
    public static InputDeviceType CurrentInputDevice { get; private set; } = InputDeviceType.Keyboard;
    
    /// <summary>
    /// Detects the current input device based on input events.
    /// Works even when the game is paused (Time.timeScale = 0).
    /// </summary>
    /// <param name="cooldownDuration">Optional cooldown duration to prevent rapid switching</param>
    /// <returns>True if the device was switched, false otherwise</returns>
    public static bool DetectCurrentInputDevice(float cooldownDuration = DEFAULT_DEVICE_SWITCH_COOLDOWN)
    {
        // Don't switch devices if we're within the cooldown period
        // Using unscaledTime to ensure this works even when the game is paused (Time.timeScale = 0)
        if (Time.unscaledTime - lastInputDeviceSwitch < cooldownDuration)
            return false;
            
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
                // Get input values for debugging
                Vector2 leftStickValue = Gamepad.current.leftStick.ReadValue();
                Vector2 rightStickValue = Gamepad.current.rightStick.ReadValue();
                bool buttonSouthPressed = Gamepad.current.buttonSouth.isPressed;
                bool buttonNorthPressed = Gamepad.current.buttonNorth.isPressed;
                bool buttonEastPressed = Gamepad.current.buttonEast.isPressed;
                bool buttonWestPressed = Gamepad.current.buttonWest.isPressed;
                float leftTriggerValue = Gamepad.current.leftTrigger.ReadValue();
                float rightTriggerValue = Gamepad.current.rightTrigger.ReadValue();
                bool leftShoulderPressed = Gamepad.current.leftShoulder.isPressed;
                bool rightShoulderPressed = Gamepad.current.rightShoulder.isPressed;
                
                // Look for significant stick movement or button presses
                if (Vector2.SqrMagnitude(leftStickValue) > STICK_THRESHOLD ||
                    Vector2.SqrMagnitude(rightStickValue) > STICK_THRESHOLD ||
                    buttonSouthPressed || buttonNorthPressed ||
                    buttonEastPressed || buttonWestPressed ||
                    leftTriggerValue > 0.5f || rightTriggerValue > 0.5f ||
                    leftShoulderPressed || rightShoulderPressed)
                {
                    shouldSwitchToGamepad = true;
                    
                    // No logging for detected input
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
                
                // No logging for detected input
            }
        }
        else if (Mouse.current != null && Mouse.current.wasUpdatedThisFrame)
        {            
            // Check for significant mouse movement or clicks
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            bool leftButtonPressed = Mouse.current.leftButton.isPressed;
            bool rightButtonPressed = Mouse.current.rightButton.isPressed;
            
            if (mouseDelta.sqrMagnitude > MOUSE_MOVEMENT_THRESHOLD ||
                leftButtonPressed || rightButtonPressed)
            {
                shouldSwitchToKeyboard = true;
                
                // No logging for detected input
            }
        }
        
        bool deviceSwitched = false;
        
        // No logging for input detection
        
        // Prioritize gamepad input if both are detected in the same frame
        if (shouldSwitchToGamepad && CurrentInputDevice != InputDeviceType.Gamepad)
        {
            UpdateInputDeviceType(InputDeviceType.Gamepad);
            lastInputDeviceSwitch = Time.unscaledTime;
            deviceSwitched = true;
        }
        else if (shouldSwitchToKeyboard && !shouldSwitchToGamepad && CurrentInputDevice != InputDeviceType.Keyboard)
        {
            UpdateInputDeviceType(InputDeviceType.Keyboard);
            lastInputDeviceSwitch = Time.unscaledTime;
            deviceSwitched = true;
        }
        
        return deviceSwitched;
    }
    
    /// <summary>
    /// Updates the current input device type and triggers the change event.
    /// </summary>
    /// <param name="deviceType">The new input device type</param>
    public static void UpdateInputDeviceType(InputDeviceType deviceType)
    {
        if (CurrentInputDevice != deviceType)
        {
            Debug.Log($"[InputDeviceDetector] Input device changed from {CurrentInputDevice} to {deviceType}");
            CurrentInputDevice = deviceType;
            OnInputDeviceChanged?.Invoke(deviceType);
        }
    }
}
