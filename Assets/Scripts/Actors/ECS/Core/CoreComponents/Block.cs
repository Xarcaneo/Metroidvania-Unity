using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : CoreComponent
{
    [Range(-180f, 180f)] public float MinAngle;
    [Range(-180f, 180f)] public float MaxAngle;

    public bool IsBetween(float angle)
    {
        if (MaxAngle > MinAngle)
        {
            return angle >= MinAngle && angle <= MaxAngle;
        }

        return (angle >= MinAngle && angle <= 180f) || (angle <= MaxAngle && angle >= -180f);
    }

    private float DetermineDamageSourceDirection(GameObject source)
    {
        return 0.0f; /*CombatUtilities.AngleFromFacingDirection(transform, source.transform, movement.FacingDirection);*/
    }
}
