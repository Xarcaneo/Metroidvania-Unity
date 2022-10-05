using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event Action onInteractionCompleted;

    public bool isInteractionCompleted = true;
    public bool canInteract = true;

    public virtual void Interact()
    {

    }

    protected virtual void IsInteractionCompleted(bool value)
    {
    }

    protected void CallInteractionCompletedEvent() => onInteractionCompleted();
}
