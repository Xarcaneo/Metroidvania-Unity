using PixelCrushers;
using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles trigger functionality for gate control and state management
/// </summary>
public class Trigger : Interactable
{
    #region Serialized Fields
    [SerializeField] 
    [Tooltip("Unique ID for this trigger, used to identify connected gates")]
    private int m_triggerID;
    #endregion

    #region Private Fields
    private bool m_triggerState;
    private List<Gate> m_connectedGates;
    private Animator m_animator;
    private Movement m_playerMovement;

    // Define the possible states of the trigger
    private enum TriggerState { IdleOn, TurningOff, IdleOff, TurningOn }
    private TriggerState m_currentState;

    // Animation parameter names
    private const string IDLE_ON_PARAM = "IdleOn";
    private const string TURNING_OFF_PARAM = "TurningOff";
    private const string IDLE_OFF_PARAM = "IdleOff";
    private const string TURNING_ON_PARAM = "TurningOn";
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeComponents();
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        
        InitializeState();
        InitializeConnectedGates();
        InitializePlayerComponents();
    }

    private void OnEnable()
    {
        // Subscribe to player spawn event if needed
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onPlayerSpawned += CachePlayerComponents;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from player spawn event
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onPlayerSpawned -= CachePlayerComponents;
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        
        if (m_triggerID <= 0)
        {
            Debug.LogWarning($"[{gameObject.name}] Trigger ID should be greater than 0!");
        }
    }
    #endregion

    #region Public Methods
    public override void Interact()
    {
        if (!ValidateComponents()) return;

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

    #region Private Methods
    private void InitializeComponents()
    {
        m_animator = GetComponent<Animator>();
        if (m_animator == null)
        {
            Debug.LogError($"[{gameObject.name}] Animator component is missing!");
        }
    }

    private void InitializePlayerComponents()
    {
        if (Player.Instance != null)
        {
            CachePlayerComponents();
        }
    }

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

    private void InitializeState()
    {
        m_triggerState = DialogueLua.GetVariable("Trigger." + m_triggerID).asBool;

        if (m_triggerState)
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

    private void InitializeConnectedGates()
    {
        // Get references to connected gates using triggerID
        m_connectedGates = new List<Gate>(FindObjectsOfType<Gate>());
        m_connectedGates.RemoveAll(gate => gate.m_gateID != m_triggerID);

        if (m_connectedGates.Count == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] No gates found with Trigger ID: {m_triggerID}");
        }
    }

    private bool ValidateComponents()
    {
        if (m_animator == null)
        {
            Debug.LogError($"[{gameObject.name}] Animator component is missing!");
            return false;
        }

        if (Player.Instance == null)
        {
            Debug.LogError($"[{gameObject.name}] Player instance is null!");
            return false;
        }

        if (m_playerMovement == null)
        {
            CachePlayerComponents();
            if (m_playerMovement == null)
            {
                Debug.LogError($"[{gameObject.name}] Player Movement component is missing!");
                return false;
            }
        }

        return true;
    }

    private void AlignPlayerWithTrigger()
    {
        if (Player.Instance != null)
        {
            Player.Instance.gameObject.transform.position = 
                new Vector3(transform.position.x, Player.Instance.gameObject.transform.position.y, 0.0f);
        }
    }

    private void ChangeState(TriggerState newState)
    {
        if (!ValidateComponents()) return;

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
                DialogueLua.SetVariable("Trigger." + m_triggerID, true);
                GameEvents.Instance?.TriggerStateChanged(m_triggerID);
                break;
            case TriggerState.TurningOff:
                m_animator.SetBool(TURNING_OFF_PARAM, true);
                break;
            case TriggerState.IdleOff:
                m_animator.SetBool(IDLE_OFF_PARAM, true);
                DialogueLua.SetVariable("Trigger." + m_triggerID, false);
                GameEvents.Instance?.TriggerStateChanged(m_triggerID);
                break;
            case TriggerState.TurningOn:
                m_animator.SetBool(TURNING_ON_PARAM, true);
                break;
        }
    }
    #endregion

    #region Animation Events
    private void OnAnimationTrigger()
    {
        if (!ValidateComponents()) return;

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
