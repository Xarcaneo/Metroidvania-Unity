using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDetector : CoreComponent
{
    public float detectionWidth;    // width of the detection area
    public float detectionHeight;   // height of the detection area
    public LayerMask entityLayer;   // layer(s) of the entity to detect
    public LayerMask obstacleLayer; // layer(s) of obstacles (e.g. walls) that can block the entity's view
    public int entityToRight;

    public bool EntityInRange()
    {
        // Check for any collider in the detection area that matches the entityLayer
        Collider2D entityCollider = Physics2D.OverlapBox(transform.position, new Vector2(detectionWidth, detectionHeight), 0, entityLayer);

        if (entityCollider != null)
        {
            // check if there is a wall blocking the entity's view of the detector
            RaycastHit2D hit = Physics2D.Linecast(transform.position, entityCollider.transform.position, obstacleLayer);

            if (hit.collider != null)
            {
                // there is an obstacle between the entity and detector, do not detect entity
                return false;
            }
            else
            {
                // entity is within range and not blocked by an obstacle
                entityToRight = entityCollider.transform.position.x > transform.position.x ? 1 : -1;
                return true;
            }
        }
        else
        {
            // entity is not within range
            return  false;
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the detection area
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector2(detectionWidth, detectionHeight));

        // Check for any collider in the detection area that matches the entityLayer
        Collider2D entityCollider = Physics2D.OverlapBox(transform.position, new Vector2(detectionWidth, detectionHeight), 0, entityLayer);

        if (entityCollider != null)
        {
            // Draw a line to the detected entity
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, entityCollider.transform.position);

            // Check if there is an obstacle blocking the view
            RaycastHit2D hit = Physics2D.Linecast(transform.position, entityCollider.transform.position, obstacleLayer);
            if (hit.collider != null)
            {
                // Draw a red line indicating the obstacle
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, hit.point);
            }
        }
    }
}
