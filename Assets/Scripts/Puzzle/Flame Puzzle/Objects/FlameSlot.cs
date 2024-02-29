using System;
using UnityEngine;
using UnityEngine.Events;

public class FlameSlot : MonoBehaviour, IInteractable
{
    private bool isActivated = false;

    // Reference to your Animator component
    private Animator animator;

    // Event to be invoked when the object is activated
    public event Action IsActivated;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        if (!isActivated)
        {
            // Trigger the animation if an Animator component is attached
            if (animator != null)
            {
                animator.SetBool("Activate", true);
            }

            // Invoke the onActivated event
            IsActivated?.Invoke();

            // Set the activation status to true
            isActivated = true;
        }
    }
}
