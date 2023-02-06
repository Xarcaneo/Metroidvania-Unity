using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractHandler : MonoBehaviour
{
    [SerializeField] protected Interactable interactableObject;
    [SerializeField] protected PictogramHandler pictogramHandler;

    private Collider2D m_collider;

    protected bool playerInRange = false;

    private void Start()
    {
        m_collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        try
        {
            GameEvents.Instance.onPlayerInteractTrigger += PlayerInteract;
            interactableObject.onInteractionCompleted += OnInteractionCompleted;
            Player.Instance.Core.GetCoreComponent<ItemDetector>().onItemDetected += DisableEnableInteraction;
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
            Player.Instance.Core.GetCoreComponent<ItemDetector>().onItemDetected -= DisableEnableInteraction;
        }
        catch
        {
        }
    }

    private void PlayerInteract(bool isInteracting)
    {
        if (playerInRange && isInteracting 
            && Player.Instance.StateMachine.CurrentState == Player.Instance.IdleState 
            && interactableObject.canInteract)
        {
            interactableObject.canInteract = false;
            pictogramHandler.HidePictogram();
            interactableObject.Interact();
        }
    }

    private void OnInteractionCompleted()
    {
        interactableObject.canInteract = true;
        ShowInteractPictogram();
    }

    protected void ShowInteractPictogram()
    {
        if (playerInRange && interactableObject.canInteract && interactableObject.canInteract)
            pictogramHandler.ShowPictogram(0);
    }

    private void DisableEnableInteraction(bool isItemDetected)
    {
        if (isItemDetected) m_collider.enabled = false;
        else m_collider.enabled = true;

        //if (isItemDetected)
        //{
        //    pictogramHandler.HidePictogram();
        //    interactableObject.canInteract = false;
        //}
        //else
        //{
        //    if (interactableObject.isInteractionCompleted)
        //    {
        //        if (playerInRange)
        //            pictogramHandler.ShowPictogram(0);

        //        interactableObject.canInteract = true;
        //    }
        //}
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