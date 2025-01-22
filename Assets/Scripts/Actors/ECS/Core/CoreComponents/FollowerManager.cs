using System.Collections.Generic;
using UnityEngine;

public class FollowerManager : CoreComponent
{
    #region Follower Management
    [Header("Follower Settings")]
    [SerializeField] private Transform target; // The target that followers will follow
    [SerializeField] private Transform scaleTarget; // The target transform to track scale for followers
    [SerializeField] private FollowerBehavior followerPrefab; // Prefab for the follower object
    [SerializeField] private int initialFollowerCount = 5; // Initial number of followers to instantiate
    [SerializeField] private bool initializeOnStart = true; // Flag to determine if followers are initialized on start

    private List<FollowerBehavior> followers = new List<FollowerBehavior>(); // List to keep track of all followers
    #endregion

    #region Unity Lifecycle
    protected override void Awake()
    {
        base.Awake(); // Call the base class Awake method

        if (followerPrefab == null)
        {
            Debug.LogError("Follower prefab is not assigned in the inspector.");
            return;
        }

        if (initializeOnStart)
        {
            InitializeFollowers();
        }
    }
    #endregion

    #region Follower Initialization
    /// <summary>
    /// Initializes the specified number of followers.
    /// </summary>
    private void InitializeFollowers()
    {
        for (int i = 0; i < initialFollowerCount; i++)
        {
            AddFollower();
        }
    }

    /// <summary>
    /// Instantiates a new follower, sets its target and scale target, and adds it to the list.
    /// </summary>
    public void AddFollower()
    {
        FollowerBehavior newFollower = Instantiate(followerPrefab, transform);
        followers.Add(newFollower);

        // Set the target and scale target for the new follower
        if (newFollower != null)
        {
            if (target != null)
                newFollower.SetTarget(target);

            if (scaleTarget != null)
                newFollower.SetScaleTarget(scaleTarget);
        }
    }

    /// <summary>
    /// Removes a follower from the list and destroys it.
    /// </summary>
    /// <param name="follower">The follower to remove.</param>
    public void RemoveFollower(FollowerBehavior follower)
    {
        if (followers.Contains(follower))
        {
            followers.Remove(follower);
            Destroy(follower.gameObject);
        }
    }
    #endregion

    #region Utility
    /// <summary>
    /// Clears all followers from the list and destroys them.
    /// </summary>
    public void ClearFollowers()
    {
        foreach (FollowerBehavior follower in followers)
        {
            Destroy(follower.gameObject);
        }
        followers.Clear();
    }
    #endregion
}
