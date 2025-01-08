using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data container for enemy chase state configuration.
/// Controls how fast the enemy moves when chasing a target.
/// </summary>
[CreateAssetMenu(fileName = "newChaseStateData", menuName = "Data/State Data/Chase State")]
public class D_ChaseState : D_BaseState
{
    [Header("Chase Settings")]
    [Tooltip("Speed at which the enemy moves while chasing")]
    [Min(0)]
    public float chaseSpeed = 3f;

    private void OnValidate()
    {
        chaseSpeed = Mathf.Max(0f, chaseSpeed);
    }
}
