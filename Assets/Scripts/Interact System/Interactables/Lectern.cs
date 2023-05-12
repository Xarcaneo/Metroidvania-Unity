using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lectern : Interactable
{
    [SerializeField] string dialogue_ID;

    private bool isInteracting = false;

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
        Anim.Play("Open");
        DialogueManager.StartConversation(dialogue_ID);
        isInteracting = true;
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
