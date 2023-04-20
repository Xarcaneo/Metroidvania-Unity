using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] private int m_gateID;
    private bool m_GateState;

    public Animator gateAnimator; // reference to the Animator component

    // Define the possible states of the gate
    enum GateState { IdleOpen, Closing, IdleClose, Opening };
    GateState currentState;

    // Define constants for the animation boolean parameter names
    const string idleOpenParam = "IdleOpen";
    const string closingParam = "Closing";
    const string idleCloseParam = "IdleClose";
    const string openingParam = "Opening";

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        gateAnimator = GetComponent<Animator>();
        m_GateState = DialogueLua.GetVariable("Trigger_" + m_gateID).asBool;

        if (m_GateState)
        {
            currentState = GateState.IdleOpen;
            gateAnimator.SetBool(idleOpenParam, true);
        }
        else
        {
            currentState = GateState.IdleClose;
            gateAnimator.SetBool(idleCloseParam, true);
        }
    }

    private void OnEnable() => GameEvents.Instance.onTriggerStateChanged += TriggerStateChanged;
    private void OnDisable() => GameEvents.Instance.onTriggerStateChanged -= TriggerStateChanged;

    private void TriggerStateChanged(int triggerID)
    {
        Debug.Log("ENTER");
        if (triggerID == m_gateID)
        {
            Debug.Log("ENTER2");
            m_GateState = DialogueLua.GetVariable("Trigger_" + m_gateID).asBool;

            if (m_GateState)
            {
                OpenGate();
            }
            else
            {
                CloseGate();
            }
        }
    }

    // Method to open the gate
    public void OpenGate()
    {
        if (currentState == GateState.IdleClose)
        {
            ChangeState(GateState.Opening);
        }
    }

    // Method to close the gate
    public void CloseGate()
    {
        if (currentState == GateState.IdleOpen)
        {
            ChangeState(GateState.Closing);
        }
    }

    // Method to change the current state of the gate and apply the animation boolean parameter
    void ChangeState(GateState newState)
    {
        // Reset the animation boolean parameter to false for the previous state
        switch (currentState)
        {
            case GateState.IdleOpen:
                gateAnimator.SetBool(idleOpenParam, false);
                break;
            case GateState.Closing:
                gateAnimator.SetBool(closingParam, false);
                break;
            case GateState.IdleClose:
                gateAnimator.SetBool(idleCloseParam, false);
                break;
            case GateState.Opening:
                gateAnimator.SetBool(openingParam, false);
                break;
        }

        currentState = newState;

        // Set the animation boolean parameter based on the new state
        switch (currentState)
        {
            case GateState.IdleOpen:
                gateAnimator.SetBool(idleOpenParam, true);
                break;
            case GateState.Closing:
                gateAnimator.SetBool(closingParam, true);
                break;
            case GateState.IdleClose:
                gateAnimator.SetBool(idleCloseParam, true);
                break;
            case GateState.Opening:
                gateAnimator.SetBool(openingParam, true);
                break;
        }
    }

    void OnAnimationTrigger()
    {
        // Transition to the new state based on the current state
        if (currentState == GateState.Closing)
        {
            ChangeState(GateState.IdleClose);
        }
        else if (currentState == GateState.Opening)
        {
            ChangeState(GateState.IdleOpen);
        }
    }
}
