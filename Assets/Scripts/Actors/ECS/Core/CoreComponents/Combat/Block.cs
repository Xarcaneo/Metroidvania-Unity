using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : CoreComponent
{
    // The minimum and maximum angles at which this object can block attacks
    [Range(-180f, 180f)] public float MinAngle;
    [Range(-180f, 180f)] public float MaxAngle;

    // Indicates whether this object is currently blocking an attack
    public bool isBlocking = false;

    // The Movement component of the object, used to determine its facing direction
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    // Checks whether the source GameObject is within the blocking angle of this object
    public bool IsBetween(Entity source)
    {
        // Calculate the angle between the facing direction of this object and the source
        float angle = AngleFromFacingDirection(core.Parent.transform, source.transform, Movement.FacingDirection);

        if (MaxAngle > MinAngle)
        {
            // If the angle range is positive (e.g. 30 to 60), check if the angle is within that range
            return angle >= MinAngle && angle <= MaxAngle;
        }

        // If the angle range is negative (e.g. -30 to -60), check if the angle is within the two sub-ranges (-180 to max angle and min angle to 180)
        return (angle >= MinAngle && angle <= 180f) || (angle <= MaxAngle && angle >= -180f);
    }

    // Calculates the angle between the facing direction of the receiver and the source
    private float AngleFromFacingDirection(Transform receiver, Transform source, int direction)
    {
        return Vector2.SignedAngle(Vector2.right * direction,
            source.position - receiver.position) * direction;
    }
}
