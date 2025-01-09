using PixelCrushers.DialogueSystem;
using UnityEngine;
using System.Collections;

/// <summary>
/// Controls a gate that can be opened or closed based on trigger states.
/// Manages animations and state persistence for gate objects.
/// </summary>
public class Gate : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Unique identifier for this gate")]
    public string m_gateID;
    #endregion

    #region Private Fields
    /// <summary>
    /// Prefix for state variables in Lua, must match InteractableState.StatePrefix
    /// </summary>
    private const string StatePrefix = "State.";

    /// <summary>
    /// Current state of the gate from DialogueLua
    /// </summary>
    private bool m_gateState;

    /// <summary>
    /// Flag indicating if the current animation has completed
    /// </summary>
    public bool isEventCompleted = false;

    /// <summary>
    /// Reference to the gate's animator component
    /// </summary>
    private Animator m_animator;

    /// <summary>
    /// Cached reference to GameEvents instance
    /// </summary>
    private GameEvents m_gameEvents;

    /// <summary>
    /// Possible states for the gate
    /// </summary>
    private enum GateState 
    { 
        IdleOpen,
        Closing, 
        IdleClose, 
        Opening 
    }

    /// <summary>
    /// Current state of the gate
    /// </summary>
    private GateState m_currentState;

    // Animation parameter names
    private const string IDLE_OPEN_PARAM = "IdleOpen";
    private const string CLOSING_PARAM = "Closing";
    private const string IDLE_CLOSE_PARAM = "IdleClose";
    private const string OPENING_PARAM = "Opening";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the gate's state and components
    /// </summary>
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        InitializeComponents();
        LoadGateState();
    }

    /// <summary>
    /// Subscribes to trigger state change events
    /// </summary>
    private void OnEnable()
    {
        m_gameEvents = GameEvents.Instance;
        if (m_gameEvents != null)
        {
            m_gameEvents.onTriggerStateChanged += TriggerStateChanged;
        }
    }

    /// <summary>
    /// Unsubscribes from trigger state change events
    /// </summary>
    private void OnDisable()
    {
        if (m_gameEvents != null)
        {
            m_gameEvents.onTriggerStateChanged -= TriggerStateChanged;
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Opens the gate if it's currently closed
    /// </summary>
    public void OpenGate()
    {
        if (m_currentState == GateState.IdleClose || m_currentState == GateState.Closing)
        {
            ChangeState(GateState.Opening);
        }
    }

    /// <summary>
    /// Closes the gate if it's currently open
    /// </summary>
    public void CloseGate()
    {
        if (m_currentState == GateState.IdleOpen || m_currentState == GateState.Opening)
        {
            ChangeState(GateState.Closing);
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initializes and caches required components
    /// </summary>
    private void InitializeComponents()
    {
        m_animator = GetComponent<Animator>();
        if (m_animator == null)
        {
            Debug.LogError($"[{gameObject.name}] Animator component is missing!");
        }
    }

    /// <summary>
    /// Loads the gate's state from DialogueLua
    /// </summary>
    private void LoadGateState()
    {
        if (!ValidateComponents()) return;

        try
        {
            m_gateState = DialogueLua.GetVariable($"{StatePrefix}{m_gateID}").asBool;
            m_currentState = m_gateState ? GateState.IdleOpen : GateState.IdleClose;
            m_animator.SetBool(m_gateState ? IDLE_OPEN_PARAM : IDLE_CLOSE_PARAM, true);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[{gameObject.name}] Error loading gate state: {e.Message}");
        }
    }

    /// <summary>
    /// Handles trigger state changes
    /// </summary>
    /// <param name="triggerID">ID of the trigger that changed state</param>
    private void TriggerStateChanged(string triggerID)
    {
        if (!ValidateComponents()) return;

        if (triggerID == m_gateID)
        {
            try
            {
                bool newState = DialogueLua.GetVariable($"{StatePrefix}{m_gateID}").asBool;
                
                // Only reset event completion if state actually changes
                if (newState != m_gateState)
                {
                    isEventCompleted = false;
                    m_gateState = newState;
                    
                    if (m_gateState)
                    {
                        OpenGate();
                    }
                    else
                    {
                        CloseGate();
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[{gameObject.name}] Error getting gate state: {e.Message}");
            }
        }
    }

    /// <summary>
    /// Changes the gate's state and updates animations
    /// </summary>
    /// <param name="newState">New state to transition to</param>
    private void ChangeState(GateState newState)
    {
        if (!ValidateComponents()) return;

        // Reset current state's animation parameter
        switch (m_currentState)
        {
            case GateState.IdleOpen:
                m_animator.SetBool(IDLE_OPEN_PARAM, false);
                break;
            case GateState.Closing:
                m_animator.SetBool(CLOSING_PARAM, false);
                break;
            case GateState.IdleClose:
                m_animator.SetBool(IDLE_CLOSE_PARAM, false);
                break;
            case GateState.Opening:
                m_animator.SetBool(OPENING_PARAM, false);
                break;
        }

        // Set new state's animation parameter
        switch (newState)
        {
            case GateState.IdleOpen:
                m_animator.SetBool(IDLE_OPEN_PARAM, true);
                break;
            case GateState.Closing:
                m_animator.SetBool(CLOSING_PARAM, true);
                break;
            case GateState.IdleClose:
                m_animator.SetBool(IDLE_CLOSE_PARAM, true);
                break;
            case GateState.Opening:
                m_animator.SetBool(OPENING_PARAM, true);
                break;
        }

        m_currentState = newState;
    }

    /// <summary>
    /// Called by animation events when a transition completes
    /// </summary>
    private void OnAnimationTrigger()
    {
        if (m_currentState == GateState.Closing)
        {
            ChangeState(GateState.IdleClose);
        }
        else if (m_currentState == GateState.Opening)
        {
            ChangeState(GateState.IdleOpen);
        }

        isEventCompleted = true;
    }

    /// <summary>
    /// Validates that all required components are present
    /// </summary>
    /// <returns>True if all components are valid, false otherwise</returns>
    private bool ValidateComponents()
    {
        if (m_animator == null)
        {
            Debug.LogError($"[{gameObject.name}] Animator component is missing!");
            return false;
        }

        return true;
    }
    #endregion
}
