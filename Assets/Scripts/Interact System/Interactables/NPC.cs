using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactable
{
    [SerializeField] private string m_dialogueID;

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

    public override void Interact()
    {
        DialogueManager.StartConversation(m_dialogueID);
    }

    protected override void IsInteractionCompleted(bool value)
    {
        if (!value)
        {
            CallInteractionCompletedEvent(); 
        }
    }
}
