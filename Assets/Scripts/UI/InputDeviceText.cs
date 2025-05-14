using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;

/// <summary>
/// Component that displays the current input binding for multiple actions based on the current input device.
/// Automatically updates when the input device changes.
/// Supports replacing keys in text with format <inputKey> where inputKey matches a key in the input bindings list.
/// </summary>
public class InputDeviceText : MonoBehaviour
{
    [System.Serializable]
    public class InputBinding
    {
        [Tooltip("The key to replace in the text (without the angle brackets)")]
        public string key;
        
        [Tooltip("Reference to the input action to display")]
        public InputActionReference actionReference;
        
        [Tooltip("Path filter for keyboard/mouse (e.g., '<Keyboard>/1'). Leave empty for auto-detection.")]
        public string keyboardPath = "";
        
        [Tooltip("Path filter for gamepad (e.g., '<Gamepad>/buttonWest'). Leave empty for auto-detection.")]
        public string gamepadPath = "";
    }
    
    [Header("Input Settings")]
    [Tooltip("List of input bindings to display")]
    [SerializeField] private List<InputBinding> inputBindings = new List<InputBinding>();
    
    [Header("UI Settings")]
    [Tooltip("The TextMeshProUGUI component to update")]
    [SerializeField] private TextMeshProUGUI tmpText;
    
    [Tooltip("The original text with input markers <inputKey>")]
    [SerializeField] private string originalText;

    private void Awake()
    {
        // If no text component is assigned, try to get one from this GameObject
        if (tmpText == null)
        {
            tmpText = GetComponent<TextMeshProUGUI>();
        }
        
        // Store the original text if not already set
        if (string.IsNullOrEmpty(originalText) && tmpText != null)
        {
            originalText = tmpText.text;
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
        // Unsubscribe from input device change events
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
    /// Updates the displayed text based on the current input device.
    /// Replaces all input markers in the original text with their corresponding bindings.
    /// </summary>
    /// <param name="deviceType">The current input device type</param>
    public void UpdateTextForInputDevice(InputDeviceDetector.InputDeviceType deviceType)
    {
        if (tmpText == null)
        {
            return;
        }
        
        if (string.IsNullOrEmpty(originalText))
        {
            originalText = tmpText.text;
        }
        
        // Start with the original text
        string processedText = originalText;
        
        // Replace each input marker with its binding
        foreach (var binding in inputBindings)
        {
            if (binding.actionReference == null || binding.actionReference.action == null || string.IsNullOrEmpty(binding.key))
            {
                continue;
            }
            
            // Get the appropriate binding path based on the current device
            string bindingPath = deviceType == InputDeviceDetector.InputDeviceType.Keyboard ? 
                binding.keyboardPath : binding.gamepadPath;
               
            string bindingText = GetBindingText(binding.actionReference, deviceType, bindingPath);
            
            // Check if the key already includes angle brackets
            string key = binding.key;
            if (key.StartsWith("<") && key.EndsWith(">"))
            {
                // Remove the brackets
                key = key.Substring(1, key.Length - 2);
            }
            
            string marker = $"<{key}>";
            
            // Replace marker with binding text if it exists in the text
            
            // Replace all occurrences of the marker with the binding text
            processedText = processedText.Replace(marker, bindingText);
        }
        
        // Set the processed text
        SetText(processedText);
    }
    
    /// <summary>
    /// Gets the binding text for the specified action and device type
    /// </summary>
    /// <param name="actionReference">The input action reference to get binding for</param>
    /// <param name="deviceType">The current input device type</param>
    /// <param name="bindingPath">Specific binding path to match, or empty for auto-detection</param>
    /// <returns>A formatted string representing the binding</returns>
    private string GetBindingText(InputActionReference actionReference, InputDeviceDetector.InputDeviceType deviceType, string bindingPath = "")
    {
        if (actionReference == null || actionReference.action == null)
        {
            return "?";
        }
        
        InputAction action = actionReference.action;
        var bindings = action.bindings;
        string bindingString = "?";
        
        // Case 1: Use specific binding path if provided
        if (!string.IsNullOrEmpty(bindingPath))
        {
            foreach (var binding in bindings)
            {
                if (binding.effectivePath.Contains(bindingPath))
                {
                    bindingString = InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                    string shortName = deviceType == InputDeviceDetector.InputDeviceType.Keyboard ? 
                        InputNameUtility.GetShortKeyboardName(bindingString) : 
                        InputNameUtility.GetShortGamepadName(bindingString);
                    return shortName;
                }
            }
        }
        
        // Case 3: Use device-specific binding
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
        
        // Case 4: If no specific binding was found, use the first binding
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
    /// Sets the original text with input markers
    /// </summary>
    /// <param name="text">Text containing input markers in the format <inputKey></param>
    public void SetOriginalText(string text)
    {
        originalText = text;
        UpdateTextForInputDevice(InputDeviceDetector.CurrentInputDevice);
    }
    
    /// <summary>
    /// Adds or updates an input binding
    /// </summary>
    /// <param name="key">The key to replace in the text (without angle brackets)</param>
    /// <param name="actionReference">The input action reference</param>
    /// <param name="keyboardPath">Path filter for keyboard/mouse, or empty for auto-detection</param>
    /// <param name="gamepadPath">Path filter for gamepad, or empty for auto-detection</param>
    public void AddInputBinding(string key, InputActionReference actionReference, string keyboardPath = "", string gamepadPath = "")
    {
        // Check if this key already exists
        InputBinding existingBinding = inputBindings.Find(b => b.key == key);
        
        if (existingBinding != null)
        {
            // Update existing binding
            existingBinding.actionReference = actionReference;
            existingBinding.keyboardPath = keyboardPath;
            existingBinding.gamepadPath = gamepadPath;
        }
        else
        {
            // Add new binding
            InputBinding newBinding = new InputBinding
            {
                key = key,
                actionReference = actionReference,
                keyboardPath = keyboardPath,
                gamepadPath = gamepadPath
            };
            
            inputBindings.Add(newBinding);
        }
        
        // Update the display
        UpdateTextForInputDevice(InputDeviceDetector.CurrentInputDevice);
    }
    
    /// <summary>
    /// Adds or updates an input binding with a single path (backward compatibility)
    /// </summary>
    public void AddInputBinding(string key, InputActionReference actionReference, string bindingPath)
    {
        // Determine if this is a keyboard or gamepad path
        string keyboardPath = "";
        string gamepadPath = "";
        
        if (!string.IsNullOrEmpty(bindingPath))
        {
            if (bindingPath.Contains("<Keyboard>") || bindingPath.Contains("<Mouse>"))
            {
                keyboardPath = bindingPath;
            }
            else if (bindingPath.Contains("<Gamepad>"))
            {
                gamepadPath = bindingPath;
            }
        }
        
        // Call the main method
        AddInputBinding(key, actionReference, keyboardPath, gamepadPath);
    }
    
    /// <summary>
    /// Removes an input binding by key
    /// </summary>
    /// <param name="key">The key to remove</param>
    public void RemoveInputBinding(string key)
    {
        inputBindings.RemoveAll(b => b.key == key);
        UpdateTextForInputDevice(InputDeviceDetector.CurrentInputDevice);
    }
    
    /// <summary>
    /// Clears all input bindings
    /// </summary>
    public void ClearInputBindings()
    {
        inputBindings.Clear();
        UpdateTextForInputDevice(InputDeviceDetector.CurrentInputDevice);
    }
}
