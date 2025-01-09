using UnityEngine;
using PixelCrushers.DialogueSystem;
using System.Collections.Generic;

/// <summary>
/// Base class for interactable objects that maintain state and have animations.
/// Provides common functionality for locks and triggers.
/// </summary>
public abstract class InteractableState : Interactable
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Unique ID for this interactable")]
    /// <summary>
    /// Unique identifier for this interactable.
    /// Used to track and persist state between game sessions.
    /// </summary>
    protected string m_stateID;

    [SerializeField]
    [Tooltip("List of gate IDs that this interactable can control")]
    /// <summary>
    /// List of gate IDs that this interactable can control.
    /// Used to manage multiple gate connections.
    /// </summary>
    protected List<string> m_connectedGateIDs = new List<string>();

    [SerializeField]
    [Tooltip("Reference to the animator component")]
    /// <summary>
    /// Reference to the animator component for animations.
    /// Controls feedback animations and state changes.
    /// </summary>
    protected Animator m_animator;
    #endregion

    #region Protected Fields
    /// <summary>
    /// Prefix for all state variables in Lua
    /// </summary>
    protected const string StatePrefix = "State.";

    /// <summary>
    /// Reference to game events system for state changes.
    /// Cached for efficient access.
    /// </summary>
    protected GameEvents m_gameEvents;

    /// <summary>
    /// List of gates connected to this interactable.
    /// These gates will be affected when the state changes.
    /// </summary>
    protected List<Gate> m_connectedGates;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Validates configuration in the Unity Editor.
    /// Ensures critical parameters are properly set.
    /// </summary>
    protected override void OnValidate()
    {
    }

    /// <summary>
    /// Initializes components by caching required references.
    /// Called when the script instance is being loaded.
    /// </summary>
    protected virtual void Awake()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Called at the beginning of the game.
    /// Initializes state and visual elements.
    /// </summary>
    protected virtual void Start()
    {
        InitializeState();
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Initializes state from Lua variables.
    /// Child classes can override this to add custom initialization logic.
    /// </summary>
    /// <returns>The current state value from Lua</returns>
    protected virtual bool InitializeStateFromLua()
    {
        bool savedState = DialogueLua.GetVariable($"{StatePrefix}{m_stateID}").asBool;
        canInteract = true; // Default to allowing interaction unless overridden
        return savedState;
    }

    /// <summary>
    /// Template method for initializing state. Called during Start.
    /// Handles the common initialization flow for all interactable states.
    /// </summary>
    protected void InitializeState()
    {
        bool savedState = InitializeStateFromLua();
        OnStateInitialized(savedState);
        UpdateVisuals(savedState);
    }

    /// <summary>
    /// Called after state is initialized from Lua.
    /// Override this to customize behavior when state is initialized.
    /// </summary>
    /// <param name="state">The initialized state value</param>
    protected virtual void OnStateInitialized(bool state)
    {
        canInteract = true; // Default behavior
    }

    /// <summary>
    /// Updates visual elements (like animations) based on state.
    /// Override this to customize how the object appears in different states.
    /// </summary>
    /// <param name="state">Current state value</param>
    protected virtual void UpdateVisuals(bool state) { }

    /// <summary>
    /// Initializes and caches required components.
    /// Called during Awake to ensure early component access.
    /// </summary>
    protected virtual void InitializeComponents()
    {
        if (m_animator == null)
        {
            m_animator = GetComponent<Animator>();
        }

        m_gameEvents = GameEvents.Instance;
        if (m_gameEvents == null)
        {
            Debug.LogWarning($"[{gameObject.name}] GameEvents instance is null!");
        }

        InitializeConnectedGates();
    }

    /// <summary>
    /// Initializes connected gates based on gate IDs.
    /// Called during initialization to cache gate references.
    /// </summary>
    protected virtual void InitializeConnectedGates()
    {
        // Find all gates in the scene
        Gate[] allGates = FindObjectsOfType<Gate>();
        
        // Filter gates based on connected IDs
        foreach (Gate gate in allGates)
        {
            if (gate != null && m_connectedGateIDs.Contains(gate.m_gateID))
            {
                if (m_connectedGates == null)
                {
                    m_connectedGates = new List<Gate>();
                }
                m_connectedGates.Add(gate);
            }
        }
    }

    /// <summary>
    /// Notifies the game system of a state change.
    /// </summary>
    protected virtual void NotifyStateChange()
    {
        if (m_gameEvents != null)
        {
            foreach (var gateID in m_connectedGateIDs)
            {
                m_gameEvents.TriggerStateChanged(gateID);
            }
        }
    }

    /// <summary>
    /// Called by animation event when animation finishes.
    /// Notifies the interaction system that the interaction is complete.
    /// </summary>
    protected virtual void OnAnimationFinished()
    {
        CallInteractionCompletedEvent();
    }

    /// <summary>
    /// Updates the state of this interactable and notifies connected gates.
    /// </summary>
    /// <param name="newState">The new state value</param>
    protected virtual void UpdateState(bool newState)
    {
        DialogueLua.SetVariable($"{StatePrefix}{m_stateID}", newState);
        NotifyGates(newState);
        OnStateChanged(newState);
    }

    /// <summary>
    /// Called when the state of this interactable changes.
    /// Override to add custom behavior when state changes.
    /// </summary>
    /// <param name="newState">The new state value</param>
    protected virtual void OnStateChanged(bool newState) { }

    /// <summary>
    /// Notifies connected gates of a state change.
    /// </summary>
    /// <param name="newState">The new state to set for connected gates</param>
    protected void NotifyGates(bool newState)
    {
        foreach (Gate gate in m_connectedGates)
        {
            if (gate != null)
            {
                DialogueLua.SetVariable($"{StatePrefix}{gate.m_gateID}", newState);
                m_gameEvents.TriggerStateChanged(gate.m_gateID);
            }
        }
    }
    #endregion
}
