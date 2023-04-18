using PixelCrushers.DialogueSystem;
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

    void Start()
    {
        gateAnimator = GetComponent<Animator>();

        m_GateState = DialogueLua.GetVariable("Trigger_" + m_gateID).asBool;

        ChangeState(m_GateState ? GateState.IdleOpen : GateState.IdleClose);
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

}
