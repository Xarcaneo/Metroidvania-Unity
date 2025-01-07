using UnityEngine;

/// <summary>
/// Handles blocking mechanics for an entity, allowing it to block attacks from specific angles.
/// </summary>
public class Block : CoreComponent
{
    #region Block Settings
    [Header("Block Angle Settings")]
    [SerializeField, Range(-180f, 180f), Tooltip("Minimum angle at which attacks can be blocked")]
    public float MinAngle;

    [SerializeField, Range(-180f, 180f), Tooltip("Maximum angle at which attacks can be blocked")]
    public float MaxAngle;

    /// <summary>
    /// Indicates whether this entity is currently attempting to block
    /// </summary>
    public bool isBlocking;
    #endregion

    #region Core Component References
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;
    #endregion

    /// <summary>
    /// Sets the blocking state of this entity
    /// </summary>
    /// <param name="blocking">True to start blocking, false to stop</param>
    public void SetBlocking(bool blocking)
    {
        isBlocking = blocking;
    }

    /// <summary>
    /// Checks if the attacking entity is within the blocking angle range
    /// </summary>
    /// <param name="source">The entity attempting to attack</param>
    /// <returns>True if the attack can be blocked based on angle, false otherwise</returns>
    public bool IsBetween(Entity source)
    {
        if (source == null || Movement == null) return false;

        // Calculate the angle between the facing direction of this object and the source
        float angle = AngleFromFacingDirection(core.Parent.transform, source.transform, Movement.FacingDirection);

        if (MaxAngle > MinAngle)
        {
            // If the angle range is positive (e.g. 30 to 60), check if the angle is within that range
            return angle >= MinAngle && angle <= MaxAngle;
        }

        // If the angle range is negative (e.g. -30 to -60), check if the angle is within the two sub-ranges
        // (-180 to max angle and min angle to 180)
        return (angle >= MinAngle && angle <= 180f) || (angle <= MaxAngle && angle >= -180f);
    }

    /// <summary>
    /// Calculates the angle between the facing direction of the receiver and the source
    /// </summary>
    private float AngleFromFacingDirection(Transform receiver, Transform source, int direction)
    {
        if (receiver == null || source == null) return 0f;

        return Vector2.SignedAngle(Vector2.right * direction,
            source.position - receiver.position) * direction;
    }
}
