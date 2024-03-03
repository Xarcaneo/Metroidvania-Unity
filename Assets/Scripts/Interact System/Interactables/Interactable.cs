using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event Action onInteractionCompleted;

    public bool canInteract = true;
    protected bool isInteracting = false;

    public virtual void Interact()
    {
        isInteracting = true;
    }

    protected virtual void IsInteractionCompleted(bool value)
    {
        isInteracting = false;
    }

    protected virtual void CallInteractionCompletedEvent() => onInteractionCompleted();
}
