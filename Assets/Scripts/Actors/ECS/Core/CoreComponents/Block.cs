using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : CoreComponent
{
    [Range(-180f, 180f)] public float MinAngle;
    [Range(-180f, 180f)] public float MaxAngle;

    public bool isBlocking = false;

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    public bool IsBetween(GameObject source)
    {
        float angle = AngleFromFacingDirection(core.Parent.transform, source.transform, Movement.FacingDirection);
 
        if (MaxAngle > MinAngle)
        {
            return angle >= MinAngle && angle <= MaxAngle;
        }

        return (angle >= MinAngle && angle <= 180f) || (angle <= MaxAngle && angle >= -180f);
    }
    private float AngleFromFacingDirection(Transform receiver, Transform source, int direction)
    {
        return Vector2.SignedAngle(Vector2.right * direction,
            source.position - receiver.position) * direction;
    }
}
