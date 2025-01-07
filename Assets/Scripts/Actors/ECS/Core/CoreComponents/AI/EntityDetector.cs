using UnityEngine;

/// <summary>
/// Detects entities within a specified area, considering line of sight and obstacles.
/// Provides visual debugging tools for entity detection and obstacle checking.
/// </summary>
public class EntityDetector : CoreComponent
{
    #region Detection Settings
    [Header("Detection Settings")]
    [SerializeField, Tooltip("Width of the detection area")]
    public float detectionWidth;

    [SerializeField, Tooltip("Height of the detection area")]
    public float detectionHeight;

    [SerializeField, Tooltip("Layer mask for entities that can be detected")]
    public LayerMask entityLayer;

    [SerializeField, Tooltip("Layer mask for obstacles that block detection")]
    public LayerMask obstacleLayer;
    #endregion

    #region State
    /// <summary>
    /// Direction to the detected entity (1 for right, -1 for left)
    /// </summary>
    public int entityToRight { get; private set; }
    #endregion

    /// <summary>
    /// Checks if an entity is within detection range and not blocked by obstacles
    /// </summary>
    /// <returns>True if entity is detected and in line of sight</returns>
    public bool EntityInRange()
    {
        // Check for entities in detection area
        Collider2D entityCollider = Physics2D.OverlapBox(
            transform.position,
            new Vector2(detectionWidth, detectionHeight),
            0,
            entityLayer
        );

        if (entityCollider == null)
        {
            return false;
        }

        // Check for obstacles between detector and entity
        RaycastHit2D hit = Physics2D.Linecast(
            transform.position,
            entityCollider.transform.position,
            obstacleLayer
        );

        if (hit.collider != null)
        {
            // View blocked by obstacle
            return false;
        }

        // Entity detected and in line of sight
        entityToRight = entityCollider.transform.position.x > transform.position.x ? 1 : -1;
        return true;
    }

    private void OnDrawGizmos()
    {
        // Draw detection area
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            transform.position,
            new Vector2(detectionWidth, detectionHeight)
        );

        // Check for entities in detection area
        Collider2D entityCollider = Physics2D.OverlapBox(
            transform.position,
            new Vector2(detectionWidth, detectionHeight),
            0,
            entityLayer
        );

        if (entityCollider == null) return;

        // Draw line to detected entity
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, entityCollider.transform.position);

        // Check and visualize obstacle detection
        RaycastHit2D hit = Physics2D.Linecast(
            transform.position,
            entityCollider.transform.position,
            obstacleLayer
        );

        if (hit.collider != null)
        {
            // Draw line to obstacle
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, hit.point);
        }
    }
}
