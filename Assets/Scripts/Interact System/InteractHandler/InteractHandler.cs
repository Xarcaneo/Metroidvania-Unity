using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractHandler : MonoBehaviour
{
    [SerializeField] protected Interactable interactableObject;
    [SerializeField] protected PictogramHandler pictogramHandler;

    protected bool playerInRange = false;

    private void OnEnable()
    {
        try
        {
            GameEvents.Instance.onPlayerInteractTrigger += PlayerInteract;
            interactableObject.onInteractionCompleted += OnInteractionCompleted;
        }
        catch
        {
        }
    }

    private void OnDisable()
    {
        try
        {
            GameEvents.Instance.onPlayerInteractTrigger -= PlayerInteract;
            interactableObject.onInteractionCompleted -= OnInteractionCompleted;
        }
        catch
        {
        }
    }

    private void PlayerInteract(bool isInteracting)
    {
        if (playerInRange && isInteracting && Player.Instance.StateMachine.CurrentState == Player.Instance.IdleState)
        {
            pictogramHandler.HidePictogram();
            interactableObject.Interact();
        }
    }

    private void OnInteractionCompleted()
    {
        ShowInteractPictogram();
    }

    protected void ShowInteractPictogram()
    {
        if (playerInRange && interactableObject.canInteract && interactableObject.isInteractionCompleted)
            pictogramHandler.ShowPictogram(0);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
            ShowInteractPictogram();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
            pictogramHandler.HidePictogram();
        }
    }
}