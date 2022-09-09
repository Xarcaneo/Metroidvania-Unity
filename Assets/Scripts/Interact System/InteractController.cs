using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractController : MonoBehaviour
{
    [SerializeField] Interactable interactableObject;

    private void CheckIfCanInteract(bool isInteracting)
    {
        if (isInteracting) interactableObject.Interact();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.InputHandler.interactPressed += CheckIfCanInteract;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.InputHandler.interactPressed -= CheckIfCanInteract;
        }
    }
}
