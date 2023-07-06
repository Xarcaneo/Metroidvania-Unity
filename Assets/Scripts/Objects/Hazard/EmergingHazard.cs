using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergingHazard : Hazard
{
    private enum SpikeState
    {
        Hidden,
        Active
    }

    [SerializeField] private SpikeState startingState = SpikeState.Hidden;
    [SerializeField] private float animationSpeed = 1.0f;

    private Animator animator;

    private const string HiddenStateName = "HiddenState";
    private const string ActiveStateName = "ActiveState";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        animator.speed = animationSpeed;

        if (startingState == SpikeState.Active)
        {
            SetActiveState();
        }
        else
        {
            SetHiddenState();
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

    // Animation event callback for state transition
    private void OnAnimationComplete()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(HiddenStateName))
        {
            SetActiveState();
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName(ActiveStateName))
        {
            SetHiddenState();
        }
    }
}
