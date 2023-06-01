using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEventTrigger : MonoBehaviour
{
    private enum EventToTrigger { StartConversation }
    [SerializeField] private EventToTrigger eventToTrigger;
    [SerializeField] private bool isOneShot = true;

    [SerializeField] private string stringVariable;

    private void OnEnable()
    {
        try
        {
            GameEvents.Instance.onDialogueTrigger += IsInteractionCompleted;
        }
        catch
        {
        }
    }

    private void OnDisable()
    {
        try
        {
            GameEvents.Instance.onDialogueTrigger -= IsInteractionCompleted;
        }
        catch
        {
        }
    }

    private void IsInteractionCompleted(bool obj)
    {
        GameEvents.Instance.DeactivatePlayerInput(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Trigger the desired event and pass the necessary variables if needed
            switch (eventToTrigger)
            {
                case EventToTrigger.StartConversation:
                    GameEvents.Instance.DeactivatePlayerInput(true);
                    DialogueManager.StartConversation(stringVariable);
                    break;
            }

            // Destroy the trigger object if it should only be triggered once
            if (isOneShot)
                Destroy(gameObject);
        }
    }
}
