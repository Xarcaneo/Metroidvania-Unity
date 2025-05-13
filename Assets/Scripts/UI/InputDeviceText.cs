using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Component that displays the current input binding for an action based on the current input device.
/// Automatically updates when the input device changes.
/// </summary>
public class InputDeviceText : MonoBehaviour
{
    [SerializeField] private InputActionReference actionReference;

    // Reference to the text component
    [SerializeField] private TextMeshProUGUI tmpText;

    private void Awake()
    {
        // Validate TextMeshProUGUI reference
        if (tmpText == null)
        {
            Debug.LogError("InputDeviceText requires a TextMeshProUGUI reference to be assigned.");
            enabled = false;
            return;
        }

        // Validate action reference
        if (actionReference == null)
        {
            Debug.LogWarning("No InputActionReference assigned to InputDeviceText.");
        }
    }

    private void OnEnable()
    {
        // Subscribe to input device change events
        InputDeviceDetector.OnInputDeviceChanged += OnInputDeviceChanged;
        UpdateTextForInputDevice(InputDeviceDetector.CurrentInputDevice);
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        InputDeviceDetector.OnInputDeviceChanged -= OnInputDeviceChanged;
    }

    private void Start()
    {
        // Initial update based on current input device
        UpdateTextForInputDevice(InputDeviceDetector.CurrentInputDevice);
    }

    /// <summary>
    /// Handles input device change events by updating the displayed text.
    /// </summary>
    /// <param name="deviceType">The new input device type</param>
    private void OnInputDeviceChanged(InputDeviceDetector.InputDeviceType deviceType)
    {
        UpdateTextForInputDevice(deviceType);
    }

    /// <summary>
    /// Updates the text based on the current input device
    /// </summary>
    public void UpdateTextForInputDevice(InputDeviceDetector.InputDeviceType deviceType)
    {
        if (actionReference == null || actionReference.action == null)
        {
            SetText("?");
            return;
        }
        
        // Get the binding display string based on the current device
        string bindingText = GetBindingText(deviceType);
        
        // Set the text to just the binding with no additional formatting
        SetText(bindingText);
        
        // Debug log to verify updates in pause menu
        if (transform.root.name.Contains("Pause") || transform.root.name.Contains("pause"))
        {
            Debug.Log($"[InputDeviceText] Updated text in pause menu: {gameObject.name} to {bindingText} for {deviceType}");
        }
    }
    
    /// <summary>
    /// Gets the binding text for the current device
    /// </summary>
    private string GetBindingText(InputDeviceDetector.InputDeviceType deviceType)
    {
        InputAction action = actionReference.action;
        
        // Get all bindings for this action
        var bindings = action.bindings;
        string bindingString = "?";
        
        // For keyboard, look for keyboard or mouse bindings
        if (deviceType == InputDeviceDetector.InputDeviceType.Keyboard)
        {
            foreach (var binding in bindings)
            {
                if (binding.effectivePath.StartsWith("<Keyboard>") || 
                    binding.effectivePath.StartsWith("<Mouse>"))
                {
                    bindingString = InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                    return InputNameUtility.GetShortKeyboardName(bindingString);
                }
            }
        }
        // For gamepad, look for gamepad bindings
        else
        {
            foreach (var binding in bindings)
            {
                if (binding.effectivePath.StartsWith("<Gamepad>"))
                {
                    bindingString = InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                    return InputNameUtility.GetShortGamepadName(bindingString);
                }
            }
        }
        
        // If no specific binding was found, use the first binding
        if (bindings.Count > 0)
        {
            bindingString = InputControlPath.ToHumanReadableString(bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            return deviceType == InputDeviceDetector.InputDeviceType.Keyboard ? 
                InputNameUtility.GetShortKeyboardName(bindingString) : 
                InputNameUtility.GetShortGamepadName(bindingString);
        }
        
        return bindingString;
    }
    

    
    /// <summary>
    /// Sets the text on the TextMeshProUGUI component
    /// </summary>
    private void SetText(string text)
    {
        if (tmpText != null)
        {
            tmpText.text = text;
        }
    }
    
    /// <summary>
    /// Updates the action reference at runtime
    /// </summary>
    public void SetActionReference(InputActionReference newActionReference)
    {
        actionReference = newActionReference;
        
        // Update the display with the new action reference
        UpdateTextForInputDevice(InputDeviceDetector.CurrentInputDevice);
    }
}
