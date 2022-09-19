using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractController : MonoBehaviour
{
    [SerializeField] Interactable interactableObject;

    private void CheckIfCanInteract(bool isInteracting)
    {
        bool isGrounded = Player.Instance.Core.CollisionSenses.Ground;

        if (Player.Instance.StateMachine.CurrentState == Player.Instance.IdleState)
            if (isInteracting && isGrounded) interactableObject.Interact();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        { 
            try { Player.Instance.InputHandler.interactPressed += CheckIfCanInteract; }
            catch { }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            try { Player.Instance.InputHandler.interactPressed -= CheckIfCanInteract; }
            catch { }
        }
    }
}
