using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : Interactable
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
        Anim.Play("Open");
        DialogueManager.StartConversation(dialogue_ID);
    }

    protected override void IsInteractionCompleted(bool value)
    {
        if (!value)
        {
            Anim.Play("Close");
        }
    }

    public void AnimationFinished()
    {
        CallInteractionCompletedEvent();
    }
}
