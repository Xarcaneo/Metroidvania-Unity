using System.Collections;
using UnityEngine;

public class PlayerFollower : FollowerBehavior
{
    private Coroutine followCoroutine; // Reference to the active follow coroutine

    protected override void Start()
    {
        base.Start();
        // Optionally initialize other settings if needed
    }

    protected override void Update()
    {
        if (target != null && followCoroutine == null)
        {
            // Start following the target if not already following
            followCoroutine = StartCoroutine(SmoothFollow(target));
        }
    }

    public override void FollowTarget(Transform target)
    {
        if (followCoroutine == null)
        {
            followCoroutine = StartCoroutine(SmoothFollow(target));
        }
    }

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
