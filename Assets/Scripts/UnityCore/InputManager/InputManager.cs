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
    public static InputManager Instance { get => _instance; }

    /// <summary>
    /// Called when the InputManager is instantiated.
    /// Ensures only one instance is active.
    /// </summary>
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    /// <summary>
    /// The PlayerInput component handling menu inputs.
    /// </summary>
    [SerializeField] public PlayerInput menuInput;

    /// <summary>
    /// Indicates whether input is currently active.
    /// </summary>
    public bool isInputActive = true;

    /// <summary>
    /// Called when the InputManager starts.
    /// Subscribes to the dialogue trigger event if GameEvents is available.
    /// </summary>
    private void Start()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onDialogueTrigger += OnDialogueTrigger;
        }
        else
        {
            Debug.LogError("GameEvents instance is not available.");
        }
    }

    /// <summary>
    /// Called when the InputManager is destroyed.
    /// Unsubscribes from the dialogue trigger event and clears the singleton instance.
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
    /// Event triggered when the menu return input is activated.
    /// </summary>
    public event Action OnMenuReturn;

    /// <summary>
    /// Event triggered when the previous tab input is activated.
    /// </summary>
    public event Action OnMenuPreviousTab;

    /// <summary>
    /// Event triggered when the next tab input is activated.
    /// </summary>
    public event Action OnMenuNextTab;

    /// <summary>
    /// Event triggered when the delete input is activated.
    /// </summary>
    public event Action OnMenuDelete;

    /// <summary>
    /// Updates the menu input state based on player input.
    /// </summary>
    private void MenuInputUpdate()
    {
        if (!isInputActive || menuInput == null) return;

        if (menuInput.actions["Return"].triggered) OnMenuReturn?.Invoke();
        else if (menuInput.actions["PreviousTab"].triggered) OnMenuPreviousTab?.Invoke();
        else if (menuInput.actions["NextTab"].triggered) OnMenuNextTab?.Invoke();
        else if (menuInput.actions["Delete"].triggered) OnMenuDelete?.Invoke();
    }

    /// <summary>
    /// Handles the dialogue trigger event to toggle input activity.
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
