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

            // Ensure the follower faces the same direction as the target
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (directionToTarget.x != 0)
            {
                Vector3 localScale = transform.localScale;
                localScale.x = Mathf.Abs(localScale.x) * (directionToTarget.x > 0 ? 1 : -1);
                transform.localScale = localScale;
            }

            // Check if within followDistance and the target isn't moving significantly
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y),
                                  new Vector2(desiredPosition.x, desiredPosition.y)) <= followDistance)
            {
                // Break the loop only if the target stops and we're close enough
                if (target.hasChanged == false)
                {
                    followCoroutine = null;
                    yield break;
                }
            }

            yield return null; // Wait until the next frame
        }
    }
}
