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
        GameEvents.Instance.onPlayerInteractTrigger += PlayerInteract;
        interactableObject.onInteractionCompleted += OnInteractionCompleted;
    }

    private void OnDisable()
    {
        GameEvents.Instance.onPlayerInteractTrigger -= PlayerInteract;
        interactableObject.onInteractionCompleted -= OnInteractionCompleted;

        StopAllCoroutines();
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
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.Core.GetCoreComponent<ItemDetector>().onItemDetected += DisableEnableInteraction;
            playerInRange = true;
            ShowInteractPictogram();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.Core.GetCoreComponent<ItemDetector>().onItemDetected -= DisableEnableInteraction;
            playerInRange = false;
            pictogramHandler.HidePictogram();
        }
    }
}