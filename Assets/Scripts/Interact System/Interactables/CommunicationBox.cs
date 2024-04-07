using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationBox : Interactable
{
    [SerializeField] string dialogue_ID;

    [SerializeField] private bool LoyalServantImage = true;

    public Animator Anim { get; private set; }

    private void Start()
    {
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

    public void AnimationFinished()
    {
        CallInteractionCompletedEvent();
    }
}
