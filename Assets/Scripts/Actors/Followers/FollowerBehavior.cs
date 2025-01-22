using System.Collections;
using UnityEngine;

/// <summary>
/// Abstract base class for implementing following behavior.
/// </summary>
public abstract class FollowerBehavior : MonoBehaviour
{
    #region Follower Properties
    [Header("Follower Settings")]
    [SerializeField] protected float followSpeed = 5.0f; // Speed of following the target
    [SerializeField] protected float followDistance = 2.0f; // Distance to maintain from the target
    [SerializeField] protected float followDelay = 0.5f; // Delay before starting to follow

    protected Transform targetToTrackScale; // Transform to track for scale synchronization

    protected Transform target;
    #endregion

    #region Abstract Methods
    /// <summary>
    /// Abstract method to define how the follower should follow the target.
    /// Derived classes must implement this method.
    /// </summary>
    /// <param name="target">The target to follow.</param>
    public abstract void FollowTarget(Transform target);

    /// <summary>
    /// Sets the target to follow.
    /// </summary>
    /// <param name="target">The target transform to follow.</param>
    public virtual void SetTarget(Transform target)
    {
        this.target = target;
    }

    /// <summary>
    /// Sets the transform whose scale will be tracked.
    /// </summary>
    /// <param name="scaleTarget">The target transform for scale tracking.</param>
    public virtual void SetScaleTarget(Transform scaleTarget)
    {
        targetToTrackScale = scaleTarget;
    }
    #endregion

    #region Unity Lifecycle
    // Optionally, derived classes can override these if needed.
    protected virtual void Start()
    {
        // Initialization logic for derived classes
    }

    protected virtual void Update()
    {
        // Frame-by-frame logic for derived classes
    }
    #endregion
}