using UnityEngine;

/// <summary>
/// Base class for all enemy state data ScriptableObjects.
/// Provides common functionality and settings for enemy states.
/// </summary>
[CreateAssetMenu(fileName = "newBaseStateData", menuName = "Data/State Data/Base State")]
public class D_BaseState : ScriptableObject
{
    [Header("State Settings")]
    [Tooltip("Whether this state can be interrupted by other states")]
    public bool isInterruptible = true;

    [Tooltip("Minimum time this state must run before it can transition")]
    [Min(0)]
    public float minStateDuration = 0f;
}
