using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lectern : Interactable
{
    [SerializeField] string dialogue_ID;

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
        Anim.Play("Open");
        DialogueManager.StartConversation(dialogue_ID);
    }

    protected override void IsInteractionCompleted(bool value)
    {
        if (!value && isInteracting)
        {
            Anim.Play("Close");
            isInteracting = false;
        }
    }

    public void AnimationFinished()
    {
        CallInteractionCompletedEvent();
    }
}
