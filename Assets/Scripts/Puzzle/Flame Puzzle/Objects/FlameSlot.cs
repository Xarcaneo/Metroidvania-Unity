using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents an interactive flame slot in the flame puzzle that can be activated or deactivated.
/// Requires an Animator component to handle visual state changes.
/// </summary>
[RequireComponent(typeof(Animator))]
public class FlameSlot : MonoBehaviour, IInteractable
{
    [Header("Slot Settings")]
    [Tooltip("Whether this flame slot can be turned off after being lit")]
    [SerializeField] private bool canBeDeactivated = false;
    
    [Tooltip("Whether this flame slot should start in an activated state")]
    [SerializeField] private bool startActivated = false;
    
    [Tooltip("Particle system to play when the flame is activated")]
    [SerializeField] private ParticleSystem activationParticles;
    
    [Tooltip("Custom prompt to show when interacting with this flame slot")]
    [SerializeField] private string customInteractionPrompt;
    
    private bool isActivated = false;
    private Animator animator;
    private static readonly int ActivateParam = Animator.StringToHash("Activate");
    
    /// <summary>
    /// Event triggered when the flame slot's state changes. Bool parameter indicates if it's activated (true) or deactivated (false).
    /// </summary>
    public event Action<bool> OnStateChanged;

    /// <summary>
    /// Gets whether the flame slot is currently activated.
    /// </summary>
    public bool IsActivated => isActivated;

    /// <summary>
    /// Determines if the flame slot can be interacted with based on its current state and settings.
    /// </summary>
    public bool CanInteract => !isActivated || canBeDeactivated;

    /// <summary>
    /// Gets the interaction prompt to display when the player can interact with this flame slot.
    /// </summary>
    public string InteractionPrompt => string.IsNullOrEmpty(customInteractionPrompt) 
        ? (isActivated ? "Deactivate Flame" : "Light Flame") 
        : customInteractionPrompt;

    /// <summary>
    /// Initializes the component by getting required references.
    /// </summary>
    private void Awake()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Sets up the initial state of the flame slot.
    /// </summary>
    private void Start()
    {
        if (startActivated)
        {
            ActivateSlot(false);
        }
    }

    /// <summary>
    /// Initializes required components and validates their presence.
    /// </summary>
    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError($"Animator component missing on {gameObject.name}");
            enabled = false;
            return;
        }
    }

    /// <summary>
    /// Handles the interaction with the flame slot, toggling its state if allowed.
    /// </summary>
    public void Interact()
    {
        if (!CanInteract) return;

        if (!isActivated)
        {
            ActivateSlot();
        }
        else if (canBeDeactivated)
        {
            DeactivateSlot();
        }
    }

    /// <summary>
    /// Activates the flame slot, playing visual effects and triggering events.
    /// </summary>
    /// <param name="playEffects">Whether to play particle effects during activation</param>
    private void ActivateSlot(bool playEffects = true)
    {
        if (isActivated) return;
        
        isActivated = true;
        animator.SetBool(ActivateParam, true);
        
        if (playEffects && activationParticles != null)
        {
            activationParticles.Play();
        }
        
        OnStateChanged?.Invoke(true);
    }

    /// <summary>
    /// Deactivates the flame slot if it's allowed to be deactivated.
    /// </summary>
    private void DeactivateSlot()
    {
        if (!isActivated || !canBeDeactivated) return;
        
        isActivated = false;
        animator.SetBool(ActivateParam, false);
        
        if (activationParticles != null)
        {
            activationParticles.Stop();
        }
        
        OnStateChanged?.Invoke(false);
    }

    /// <summary>
    /// Called when values are changed in the Unity Inspector.
    /// Updates the animator state in edit mode if startActivated is true.
    /// </summary>
    private void OnValidate()
    {
        if (startActivated && !Application.isPlaying)
        {
            animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool(ActivateParam, true);
            }
        }
    }
}
