using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : Interactable
{
    public Animator entityAnim; // reference to the Animator component

    // Define the possible states of the trigger
    enum TriggerState { IdleOn, TurningOff, IdleOff, TurningOn };
    TriggerState currentState;

    // Define constants for the animation boolean parameter names
    const string idleOnParam = "IdleOn";
    const string turningOffParam = "TurningOff";
    const string idleOffParam = "IdleOff";
    const string turningOnParam = "TurningOn";

    void Start()
    {
        entityAnim = GetComponent<Animator>();

        currentState = TriggerState.IdleOn; // set initial state to IdleOn
        entityAnim.SetBool(idleOnParam, true); // set the animation boolean parameter to true
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
                break;
            case TriggerState.TurningOff:
                entityAnim.SetBool(turningOffParam, true);
                break;
            case TriggerState.IdleOff:
                entityAnim.SetBool(idleOffParam, true);
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

        CallInteractionCompletedEvent();
    }
}
