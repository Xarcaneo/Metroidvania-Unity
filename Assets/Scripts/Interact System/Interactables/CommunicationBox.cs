using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationBox : Interactable
{
    [SerializeField] string dialogue_ID;
    [SerializeField] string communicationBox_ID;
    [SerializeField] private bool LoyalServantImage = true;

    public Animator Anim { get; private set; }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        var communicationBoxState = DialogueLua.GetVariable("CommunicationBox." + communicationBox_ID).asBool;

        if (communicationBoxState)
            canInteract = false;

        Anim = GetComponent<Animator>();
    }

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
        base.Interact();

        Anim.Play("TurnOn");
        DialogueManager.StartConversation(dialogue_ID);
        SetConversationFinished();

    }

    private void SetConversationFinished()
    {
        canInteract = false;
        DialogueLua.SetVariable("CommunicationBox." + communicationBox_ID, true);
    }

    protected override void IsInteractionCompleted(bool value)
    {
        if (!value && isInteracting)
        {
            Anim.Play("TurnOff");
            isInteracting = false;
        }
    }

    public void OnTurnOnAnimationComplete()
    {
        if (LoyalServantImage)
        {
            Anim.Play("LoyalServantIdle");
        }
    }
}
