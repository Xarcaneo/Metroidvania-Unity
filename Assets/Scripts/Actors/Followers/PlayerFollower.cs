using System.Collections;
using UnityEngine;

/// <summary>
/// A class for smoothly following a target in a 2D environment.
/// Implements smooth movement and ensures the follower faces the same direction as the target.
/// </summary>
public class PlayerFollower : FollowerBehavior
{
    /// <summary>
    /// Reference to the active follow coroutine.
    /// </summary>
    private Coroutine followCoroutine;

    /// <summary>
    /// Unity Start method.
    /// Called before the first frame update.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        // Optionally initialize other settings if needed
    }

    /// <summary>
    /// Unity Update method.
    /// Called once per frame.
    /// Starts the follow coroutine if not already running.
    /// </summary>
    protected override void Update()
    {
        if (target != null && followCoroutine == null)
        {
            // Start following the target if not already following
            followCoroutine = StartCoroutine(SmoothFollow(target));
        }
    }

    /// <summary>
    /// Initiates the following behavior for the specified target.
    /// </summary>
    /// <param name="target">The target Transform to follow.</param>
    public override void FollowTarget(Transform target)
    {
        if (followCoroutine == null)
        {
            followCoroutine = StartCoroutine(SmoothFollow(target));
        }
    }

    /// <summary>
    /// Coroutine for smoothly following the target.
    /// Continuously updates position and ensures the follower faces the same direction as the target.
    /// </summary>
    /// <param name="target">The target Transform to follow.</param>
    /// <returns>An IEnumerator for the coroutine.</returns>
    private IEnumerator SmoothFollow(Transform target)
    {
        while (true)
        {
            if (target == null)
            {
                followCoroutine = null;
                yield break;
            }

            // Calculate the desired position
            Vector3 desiredPosition = new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z // Maintain current z-axis
            );

            // Gradually move towards the desired position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

            // Synchronize follower's scale with the target's scale if provided
            if (targetToTrackScale != null)
            {
                Vector3 localScale = transform.localScale;
                localScale.x = Mathf.Abs(localScale.x) * Mathf.Sign(targetToTrackScale.localScale.x);
                transform.localScale = localScale;
            }

            yield return null; // Wait until the next frame
        }
    }
}