using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles trigger functionality for controlling connected gates and managing trigger states.
/// This class manages the interaction between player input and gate mechanisms, handling animations
/// and state transitions for both the trigger and its connected gates.
/// </summary>
public class Trigger : InteractableState
{
    #region Private Fields
    /// <summary>
    /// Current state of the trigger (on/off). Used to track and persist trigger state.
    /// </summary>
    private bool m_triggerState;

    /// <summary>
    /// Reference to player's movement component for handling player orientation.
    /// Used to ensure player faces the correct direction during interactions.
    /// </summary>
    private Movement m_playerMovement;

    /// <summary>
    /// Possible states for the trigger's animation and functionality.
    /// Controls the flow of trigger state transitions.
    /// </summary>
    private enum TriggerState { 
        /// <summary>Trigger is active and idle</summary>
        IdleOn, 
        /// <summary>Trigger is transitioning to off state</summary>
        TurningOff, 
        /// <summary>Trigger is inactive and idle</summary>
        IdleOff, 
        /// <summary>Trigger is transitioning to on state</summary>
        TurningOn 
    }
    
    /// <summary>
    /// Current state of the trigger's animation and functionality.
    /// </summary>
    private TriggerState m_currentState;

    // Animation parameter names
    /// <summary>
    /// Constants for animator parameter names to ensure consistency
    /// and prevent typos in animation calls.
    /// </summary>
    private const string IDLE_ON_PARAM = "IdleOn";
    private const string TURNING_OFF_PARAM = "TurningOff";
    private const string IDLE_OFF_PARAM = "IdleOff";
    private const string TURNING_ON_PARAM = "TurningOn";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Subscribes to player spawn events when the object becomes enabled.
    /// Ensures component references stay valid after player respawns.
    /// </summary>
    private void OnEnable()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onPlayerSpawned += CachePlayerComponents;
        }
    }

    /// <summary>
    /// Unsubscribes from player spawn events when the object becomes disabled.
    /// Prevents memory leaks and invalid event calls.
    /// </summary>
    private void OnDisable()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onPlayerSpawned -= CachePlayerComponents;
        }
    }

    /// <summary>
    /// Initializes the trigger after all objects are initialized.
    /// </summary>
    protected override void Start()
    {
        base.Start(); // Call base class initialization
        StartCoroutine(InitializeAfterFrame());
    }

    /// <summary>
    /// Performs initialization after waiting one frame to ensure proper initialization order.
    /// </summary>
    private IEnumerator InitializeAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        InitializePlayerComponents();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Handles the interaction with this trigger when activated by the player.
    /// Manages player positioning, trigger state changes, and animation transitions.
    /// </summary>
    public override void Interact()
    {
        AlignPlayerWithTrigger();
        Player.Instance.gameObject.SetActive(false);

        // Change state based on current state
        if (m_currentState == TriggerState.IdleOn)
        {
            ChangeState(TriggerState.TurningOff);
        }
        else if (m_currentState == TriggerState.IdleOff)
        {
            ChangeState(TriggerState.TurningOn);
        }
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Handles state initialization for the trigger.
    /// </summary>
    /// <param name="state">Current trigger state</param>
    protected override void OnStateInitialized(bool state)
    {
        m_triggerState = state;
        base.OnStateInitialized(state);
    }

    /// <summary>
    /// Updates trigger animations based on state.
    /// </summary>
    /// <param name="state">Current trigger state</param>
    protected override void UpdateVisuals(bool state)
    {
        if (state)
        {
            m_currentState = TriggerState.IdleOn;
            m_animator.SetBool(IDLE_ON_PARAM, true);
        }
        else
        {
            m_currentState = TriggerState.IdleOff;
            m_animator.SetBool(IDLE_OFF_PARAM, true);
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initializes player-related components if player instance exists.
    /// Called during Start after player instantiation.
    /// </summary>
    private void InitializePlayerComponents()
    {
        if (Player.Instance != null)
        {
            CachePlayerComponents();
        }
    }

    /// <summary>
    /// Caches player movement component for efficient access.
    /// Called when player spawns or during initialization.
    /// </summary>
    private void CachePlayerComponents()
    {
        if (Player.Instance?.Core != null)
        {
            m_playerMovement = Player.Instance.Core.GetCoreComponent<Movement>();
            if (m_playerMovement == null)
            {
                Debug.LogWarning($"[{gameObject.name}] Player Movement component not found!");
            }
        }
    }

    /// <summary>
    /// Aligns the player's position with the trigger.
    /// Ensures consistent interaction positioning.
    /// </summary>
    private void AlignPlayerWithTrigger()
    {
        if (Player.Instance != null)
        {
            Player.Instance.gameObject.transform.position = 
                new Vector3(transform.position.x, Player.Instance.gameObject.transform.position.y, 0.0f);
        }
    }

    /// <summary>
    /// Changes the trigger's state and updates animations accordingly.
    /// Manages state transitions and notifies connected systems.
    /// </summary>
    /// <param name="newState">The target state to transition to</param>
    private void ChangeState(TriggerState newState)
    {
        // Reset previous state
        switch (m_currentState)
        {
            case TriggerState.IdleOn:
                m_animator.SetBool(IDLE_ON_PARAM, false);
                break;
            case TriggerState.TurningOff:
                m_animator.SetBool(TURNING_OFF_PARAM, false);
                break;
            case TriggerState.IdleOff:
                m_animator.SetBool(IDLE_OFF_PARAM, false);
                break;
            case TriggerState.TurningOn:
                m_animator.SetBool(TURNING_ON_PARAM, false);
                break;
        }

        m_currentState = newState;

        // Set new state
        switch (m_currentState)
        {
            case TriggerState.IdleOn:
                m_animator.SetBool(IDLE_ON_PARAM, true);
                UpdateState(true);
                break;
            case TriggerState.TurningOff:
                m_animator.SetBool(TURNING_OFF_PARAM, true);
                break;
            case TriggerState.IdleOff:
                m_animator.SetBool(IDLE_OFF_PARAM, true);
                UpdateState(false);
                break;
            case TriggerState.TurningOn:
                m_animator.SetBool(TURNING_ON_PARAM, true);
                break;
        }
    }
    #endregion

    #region Animation Events
    /// <summary>
    /// Called by animation event when trigger state change animation completes.
    /// Handles player orientation, state updates, and gate notifications.
    /// </summary>
    private void OnAnimationTrigger()
    {
        // Update state
        if (m_currentState == TriggerState.TurningOff)
        {
            ChangeState(TriggerState.IdleOff);
        }
        else if (m_currentState == TriggerState.TurningOn)
        {
            ChangeState(TriggerState.IdleOn);
        }
            
        // Handle player
        if (m_playerMovement.FacingDirection != transform.localScale.x)
        {
            m_playerMovement.Flip();
        }

        Player.Instance.gameObject.SetActive(true);
        Player.Instance.SetPlayerStateToIdle();

        StartCoroutine(CheckConnectedGatesEventCompletion());
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// Monitors connected gates for event completion.
    /// Continues checking until all gates have completed their events.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution</returns>
    private IEnumerator CheckConnectedGatesEventCompletion()
    {
        if (m_connectedGates == null || m_connectedGates.Count == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] No connected gates to check!");
            yield break;
        }

        while (true)
        {
            bool allGatesCompleted = true;

            foreach (Gate gate in m_connectedGates)
            {
                if (gate == null)
                {
                    Debug.LogError($"[{gameObject.name}] Connected gate is null!");
                    continue;
                }

                if (!gate.isEventCompleted)
                {
                    allGatesCompleted = false;
                    break;
                }
            }

            if (allGatesCompleted)
            {
                CallInteractionCompletedEvent();
                yield break;
            }

            yield return null;
        }
    }
    #endregion
}
