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

    private bool entityInRange = false; // whether the entity is within detection range

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
                return entityInRange = false;
            }
            else
            {
                // entity is within range and not blocked by an obstacle
                entityToRight = entityCollider.transform.position.x > transform.position.x ? 1 : -1;
                return entityInRange = true;
            }
        }
        else
        {
            // entity is not within range
            return entityInRange = false;
        }
    }

    private void OnDrawGizmos()
    {
        // Draw a wire rectangle at the detection range to visualize the area
        Gizmos.color = entityInRange ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(detectionWidth, detectionHeight, 0));
    }
}
