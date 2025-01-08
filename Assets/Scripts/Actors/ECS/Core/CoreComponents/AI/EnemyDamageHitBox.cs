using UnityEngine;

/// <summary>
/// Extends DamageHitBox with enemy-specific detection capabilities.
/// Handles entity detection with obstacle awareness and provides debug visualization.
/// </summary>
public class EnemyDamageHitBox : DamageHitBox
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

    #region Debug Settings
    [Header("Debug Settings")]
    [SerializeField, Tooltip("Enable to show detection area gizmos")]
    private bool debug = false;
    #endregion

    #region State
    /// <summary>
    /// Direction to the detected entity (1 for right, -1 for left)
    /// </summary>
    public int entityToRight { get; private set; }

    /// <summary>
    /// Whether an entity is currently within detection range
    /// </summary>
    private bool entityInRange = false;
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
            return entityInRange = false;
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
            return entityInRange = false;
        }

        // Entity detected and in line of sight
        entityToRight = entityCollider.transform.position.x > transform.position.x ? 1 : -1;
        return entityInRange = true;
    }

    private void OnDrawGizmos()
    {
        if (!debug) return;

        // Draw detection area
        Gizmos.color = entityInRange ? Color.green : Color.red;
        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(detectionWidth, detectionHeight, 0)
        );
    }
}
