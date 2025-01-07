using UnityEngine;

/// <summary>
/// Data container for enemy idle state configuration.
/// Controls how long the enemy remains idle.
/// </summary>
[CreateAssetMenu(fileName = "newIdleStateData", menuName = "Data/State Data/Idle State")]
public class D_IdleState : D_BaseState
{
    [Header("Idle Duration")]
    [Tooltip("Minimum time to remain in idle state")]
    [Min(0)]
    public float minIdleTime = 1f;

    [Tooltip("Maximum time to remain in idle state")]
    [Min(0)]
    public float maxIdleTime = 2f;

    private void OnValidate()
    {
        // Ensure times are positive and min <= max
        minIdleTime = Mathf.Max(0f, minIdleTime);
        maxIdleTime = Mathf.Max(minIdleTime, maxIdleTime);
    }
}