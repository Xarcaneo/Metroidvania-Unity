using UnityEngine;

/// <summary>
/// Data container for enemy movement state configuration.
/// Controls basic movement speed.
/// </summary>
[CreateAssetMenu(fileName = "newMoveStateData", menuName = "Data/State Data/Move State")]
public class D_MoveState : D_BaseState
{
    [Header("Movement Settings")]
    [Tooltip("Speed at which the enemy moves")]
    [Min(0)]
    public float movementSpeed = 3f;

    private void OnValidate()
    {
        movementSpeed = Mathf.Max(0f, movementSpeed);
    }
}