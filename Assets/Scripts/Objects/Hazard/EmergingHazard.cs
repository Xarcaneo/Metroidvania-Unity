using System.Collections;
using UnityEngine;

public class EmergingHazard : Hazard
{
    private enum SpikeState
    {
        Hidden,
        Active
    }

    [SerializeField] private SpikeState startingState = SpikeState.Hidden;
    [SerializeField] private float activeInterval = 2.0f;  // Time in seconds spikes are active
    [SerializeField] private float hiddenDuration = 0.1f;  // Duration spikes are hidden before becoming active again

    private Animator animator;

    private const string HiddenStateName = "HiddenState";
    private const string ActiveStateName = "ActiveState";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(SpikeActivationCoroutine());
    }

    private IEnumerator SpikeActivationCoroutine()
    {
        while (true)
        {
            SetActiveState();
            yield return new WaitForSeconds(activeInterval); // Spike is active for a set duration
            SetHiddenState();
            yield return new WaitForSeconds(hiddenDuration); // Spike is hidden for a short duration
        }
    }

    private void SetHiddenState()
    {
        animator.Play(HiddenStateName);
    }

    private void SetActiveState()
    {
        animator.Play(ActiveStateName);
    }
}
