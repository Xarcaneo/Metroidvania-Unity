using System.Collections;
using UnityEngine;

/// <summary>
/// A hazard that emerges and retracts periodically, creating a timing-based challenge.
/// Uses animation states to control its visibility and collision.
/// </summary>
public class EmergingHazard : Hazard
{
    /// <summary>
    /// Represents the current state of the spikes.
    /// </summary>
    private enum SpikeState
    {
        /// <summary>
        /// Spikes are retracted and cannot damage entities.
        /// </summary>
        Hidden,

        /// <summary>
        /// Spikes are extended and can damage entities.
        /// </summary>
        Active
    }

    [Header("Timing Settings")]
    /// <summary>
    /// Duration in seconds that the spikes remain in their active/extended state.
    /// </summary>
    [SerializeField] private float activeInterval = 2.0f;

    /// <summary>
    /// Duration in seconds that the spikes remain hidden before emerging again.
    /// </summary>
    [SerializeField] private float hiddenDuration = 0.1f;

    /// <summary>
    /// Reference to the Animator component for controlling spike states.
    /// </summary>
    private Animator animator;

    // Animation state names
    private const string HiddenStateName = "HiddenState";
    private const string ActiveStateName = "ActiveState";

    /// <summary>
    /// Initializes the required components.
    /// </summary>
    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError($"[EmergingHazard] Missing Animator component on {gameObject.name}!", this);
        }
    }

    /// <summary>
    /// Starts the spike activation cycle when the object is enabled.
    /// </summary>
    private void Start()
    {
        StartCoroutine(SpikeActivationCoroutine());
    }

    /// <summary>
    /// Coroutine that handles the cyclic activation and deactivation of the spikes.
    /// </summary>
    private IEnumerator SpikeActivationCoroutine()
    {
        while (true)
        {
            SetActiveState();
            yield return new WaitForSeconds(activeInterval);
            SetHiddenState();
            yield return new WaitForSeconds(hiddenDuration);
        }
    }

    /// <summary>
    /// Sets the spikes to their hidden state using animation.
    /// </summary>
    private void SetHiddenState()
    {
        if (animator != null)
        {
            animator.Play(HiddenStateName);
        }
    }

    /// <summary>
    /// Sets the spikes to their active state using animation.
    /// </summary>
    private void SetActiveState()
    {
        if (animator != null)
        {
            animator.Play(ActiveStateName);
        }
    }
}
