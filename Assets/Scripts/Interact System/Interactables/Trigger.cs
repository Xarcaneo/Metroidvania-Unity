using PixelCrushers;
using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : Interactable
{
    [SerializeField] private int m_triggerID;
    private bool m_triggerState;

    private List<Gate> connectedGates; // List to store references to connected gates

    public Animator entityAnim; // reference to the Animator component

    // Define the possible states of the trigger
    enum TriggerState { IdleOn, TurningOff, IdleOff, TurningOn };
    TriggerState currentState;

    // Define constants for the animation boolean parameter names
    const string idleOnParam = "IdleOn";
    const string turningOffParam = "TurningOff";
    const string idleOffParam = "IdleOff";
    const string turningOnParam = "TurningOn";

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        entityAnim = GetComponent<Animator>();
        m_triggerState = DialogueLua.GetVariable("Trigger." + m_triggerID).asBool;

        if (m_triggerState)
        {
            currentState = TriggerState.IdleOn;
            entityAnim.SetBool(idleOnParam, true);
        }
        else
        {
            currentState = TriggerState.IdleOff;
            entityAnim.SetBool(idleOffParam, true);
        }

        // Get references to connected gates using triggerID
        connectedGates = new List<Gate>(FindObjectsOfType<Gate>());
        connectedGates.RemoveAll(gate => gate.m_gateID != m_triggerID);
    }

    public override void Interact()
    {
        GameEvents.Instance.DeactivatePlayerInput(true);
        Player.Instance.gameObject.transform.position = this.transform.position;
        Player.Instance.gameObject.GetComponent<Renderer>().enabled = false;

        // Change state based on current state
        if (currentState == TriggerState.IdleOn)
        {
            ChangeState(TriggerState.TurningOff);
        }
        else if (currentState == TriggerState.IdleOff)
        {
            ChangeState(TriggerState.TurningOn);
        }
    }

    // Method to change the current state of the trigger and apply the animation boolean parameter
    void ChangeState(TriggerState newState)
    {
        // Reset the animation boolean parameter to false for the previous state
        switch (currentState)
        {
            case TriggerState.IdleOn:
                entityAnim.SetBool(idleOnParam, false);
                break;
            case TriggerState.TurningOff:
                entityAnim.SetBool(turningOffParam, false);
                break;
            case TriggerState.IdleOff:
                entityAnim.SetBool(idleOffParam, false);
                break;
            case TriggerState.TurningOn:
                entityAnim.SetBool(turningOnParam, false);
                break;
        }

        currentState = newState;

        // Set the animation boolean parameter based on the new state
        switch (currentState)
        {
            case TriggerState.IdleOn:
                entityAnim.SetBool(idleOnParam, true);
                DialogueLua.SetVariable("Trigger." + m_triggerID, true);
                GameEvents.Instance.TriggerStateChanged(m_triggerID);
                break;
            case TriggerState.TurningOff:
                entityAnim.SetBool(turningOffParam, true);
                break;
            case TriggerState.IdleOff:
                entityAnim.SetBool(idleOffParam, true);
                DialogueLua.SetVariable("Trigger." + m_triggerID, false);
                GameEvents.Instance.TriggerStateChanged(m_triggerID);
                break;
            case TriggerState.TurningOn:
                entityAnim.SetBool(turningOnParam, true);
                break;
        }
    }

    // Method to handle animation trigger events
    void OnAnimationTrigger()
    {
        // Transition to the new state based on the current state
        if (currentState == TriggerState.TurningOff)
        {
            ChangeState(TriggerState.IdleOff);
        }
        else if (currentState == TriggerState.TurningOn)
        {
            ChangeState(TriggerState.IdleOn);
        }

        Player.Instance.gameObject.GetComponent<Renderer>().enabled = true;

        if (Player.Instance.Core.GetCoreComponent<Movement>().FacingDirection != this.transform.localScale.x) 
            Player.Instance.Core.GetCoreComponent<Movement>().Flip();

        GameEvents.Instance.DeactivatePlayerInput(false);


        StartCoroutine(CheckConnectedGatesEventCompletion());
    }

    IEnumerator CheckConnectedGatesEventCompletion()
    {
        while (true)
        {
            bool allGatesCompleted = true;

            foreach (Gate gate in connectedGates)
            {
                if (!gate.isEventCompleted)
                {
                    allGatesCompleted = false;
                    break;
                }
            }

            if (allGatesCompleted)
            {
                CallInteractionCompletedEvent();
                yield break; // Exit the coroutine
            }

            yield return null; // Wait for the next frame before checking again
        }
    }
}
