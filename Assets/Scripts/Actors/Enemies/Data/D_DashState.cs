using UnityEngine;

/// <summary>
/// Data container for enemy dash state configuration.
/// Handles dash movement, detection, and collision parameters.
/// </summary>
[CreateAssetMenu(fileName = "newDashStateData", menuName = "Data/State Data/Dash State")]
public class D_DashState : D_BaseState
{
    [Header("Detection Settings")]
    [Tooltip("Width of the area to detect targets")]
    [Min(0)]
    public float detectionWidth = 5f;

    [Tooltip("Height of the area to detect targets")]
    [Min(0)]
    public float detectionHeight = 3f;

    [Tooltip("Layer mask for entities that can be detected")]
    public LayerMask entityLayer;

    [Tooltip("Top position for ground detection raycast")]
    public Vector3 raycastTopPosition;

    [Tooltip("Bottom position for ground detection raycast")]
    public Vector3 raycastBottomPosition;

    [Tooltip("Distance to check for ground")]
    [Min(0)]
    public int groundRaycastDistance = 1;

    [Header("Collision Settings")]
    [Tooltip("Layers to check for collision during dash")]
    public LayerMask collisionLayer;

    [Header("Dash Parameters")]
    [Tooltip("Velocity applied during dash (x = horizontal, y = vertical)")]
    public Vector2 dashVelocity = new Vector2(30f, 0f);

    [Tooltip("Angle of the dash (normalized in-game)")]
    public Vector2 dashAngle = new Vector2(1f, 0f);

    [Tooltip("Duration of the dash in seconds")]
    [Min(0)]
    public float dashTime = 0.2f;

    private void OnValidate()
    {
        // Ensure dash parameters are valid
        dashTime = Mathf.Max(0f, dashTime);
        detectionWidth = Mathf.Max(0f, detectionWidth);
        detectionHeight = Mathf.Max(0f, detectionHeight);
        groundRaycastDistance = Mathf.Max(0, groundRaycastDistance);

        // Normalize dash angle
        if (dashAngle != Vector2.zero)
        {
            dashAngle.Normalize();
        }
        else
        {
            dashAngle = Vector2.right;
        }
    }
}
