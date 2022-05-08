using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMeleeAttackDetector : CoreComponent
{
    public LayerMask targetLayer;

    [Header("OverlapBoxParameters")]
    public Vector2 detectorSize = Vector2.one;

    [Header("Gizmo parameters")]
    public Color gizmoColor = Color.green;
    public bool showGizmos = true;

    private bool EntityDetected;

    private void Update()
    {
        var collider = Physics2D.OverlapBox((Vector2)transform.position, detectorSize, 0, targetLayer);
        EntityDetected = collider != null;
    }

    public bool GetEntityDetected()
    {
        return EntityDetected;
    }

    private void OnDrawGizmos()
    {
        if (showGizmos) 
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawCube((Vector2)transform.position, detectorSize);
        }
    }
}
