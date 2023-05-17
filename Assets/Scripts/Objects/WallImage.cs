using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallImage : MonoBehaviour
{
    [SerializeField]
    private float minAnimationTime = 1f;
    [SerializeField]
    private float maxAnimationTime = 3f;
    [SerializeField]
    private AnimationClip idleAnimation;
    [SerializeField]
    private AnimationClip animationAnimation;

    private Animator animator;
    private Coroutine animationCoroutine;

    private void Start()
    {
        animator = GetComponent<Animator>();

        // Start the animation coroutine
        animationCoroutine = StartCoroutine(PlayAnimationWithRandomInterval());
    }

    private IEnumerator PlayAnimationWithRandomInterval()
    {
        while (true)
        {
            // Play the idle animation
            animator.Play(idleAnimation.name);

            // Wait for a random interval
            float randomTime = Random.Range(minAnimationTime, maxAnimationTime);
            yield return new WaitForSeconds(randomTime);

            // Play the "animation" animation
            animator.Play(animationAnimation.name);

            // Wait for the "animation" animation to finish
            yield return new WaitForSeconds(animationAnimation.length);
        }
    }

    private void OnDisable()
    {
        // Stop the animation coroutine when the script is disabled
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
    }
}
