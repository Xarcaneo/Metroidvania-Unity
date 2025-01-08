using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class FlameSlot : MonoBehaviour, IInteractable
{
    [SerializeField] private bool canBeDeactivated = false;
    [SerializeField] private bool startActivated = false;
    [SerializeField] private ParticleSystem activationParticles;
    [SerializeField] private string customInteractionPrompt;
    
    private bool isActivated = false;
    private Animator animator;
    private static readonly int ActivateParam = Animator.StringToHash("Activate");
    
    public event Action<bool> OnStateChanged;
    public bool IsActivated => isActivated;

    public bool CanInteract => !isActivated || canBeDeactivated;
    public string InteractionPrompt => string.IsNullOrEmpty(customInteractionPrompt) 
        ? (isActivated ? "Deactivate Flame" : "Light Flame") 
        : customInteractionPrompt;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError($"Animator component missing on {gameObject.name}");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        if (startActivated)
        {
            ActivateSlot(false);
        }
    }

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
