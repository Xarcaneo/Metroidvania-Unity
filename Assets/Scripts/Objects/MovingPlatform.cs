using UnityEngine;

/// <summary>
/// Controls a platform that moves between defined waypoints.
/// Handles object parenting for items and characters standing on the platform.
/// </summary>
public class MovingPlatform : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Movement speed of the platform")]
    [Range(0.1f, 20f)]
    private float m_speed = 5f;

    [SerializeField]
    [Tooltip("Index of the starting waypoint")]
    [Min(0)]
    private int m_startingPoint;

    [SerializeField]
    [Tooltip("Array of waypoints for the platform to move between")]
    private Transform[] m_points;

    [SerializeField]
    [Tooltip("Vertical offset for collision detection")]
    [Range(0f, 5f)]
    private float m_collisionOffset = 1.8f;
    #endregion

    #region Private Fields
    /// <summary>
    /// Current target waypoint index
    /// </summary>
    private int m_currentPointIndex;

    // Constants
    private const float DISTANCE_THRESHOLD = 0.02f;
    private const string ITEM_TAG = "Item";
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes platform position
    /// </summary>
    private void Start()
    {
        InitializePlatform();
    }

    /// <summary>
    /// Updates platform movement
    /// </summary>
    private void FixedUpdate()
    {
        if (!ValidateComponents()) return;
        MovePlatform();
    }

    /// <summary>
    /// Handles collision enter events for parenting objects
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;

        // Parent items automatically
        if (collision.gameObject.CompareTag(ITEM_TAG))
        {
            collision.transform.SetParent(transform);
            return;
        }

        // Parent objects standing on top of the platform
        if (transform.position.y < collision.transform.position.y - m_collisionOffset)
        {
            collision.transform.SetParent(transform);
        }
    }

    /// <summary>
    /// Handles collision exit events for unparenting objects
    /// </summary>
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision?.transform != null)
        {
            collision.transform.SetParent(null);
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Validates that all required components are present
    /// </summary>
    /// <returns>True if all components are valid, false otherwise</returns>
    private bool ValidateComponents()
    {
        if (m_points == null || m_points.Length == 0)
        {
            Debug.LogError($"[{gameObject.name}] No waypoints assigned!");
            return false;
        }

        if (m_startingPoint >= m_points.Length)
        {
            Debug.LogError($"[{gameObject.name}] Starting point index out of range!");
            return false;
        }

        foreach (var point in m_points)
        {
            if (point == null)
            {
                Debug.LogError($"[{gameObject.name}] One or more waypoints are null!");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Initializes platform position and validates components
    /// </summary>
    private void InitializePlatform()
    {
        if (!ValidateComponents()) return;

        m_currentPointIndex = m_startingPoint;
        transform.position = m_points[m_startingPoint].position;
    }

    /// <summary>
    /// Moves the platform between waypoints
    /// </summary>
    private void MovePlatform()
    {
        // Check if we've reached the current waypoint
        if (Vector2.Distance(transform.position, m_points[m_currentPointIndex].position) < DISTANCE_THRESHOLD)
        {
            // Move to next waypoint
            m_currentPointIndex = (m_currentPointIndex + 1) % m_points.Length;
        }

        // Move towards current waypoint
        transform.position = Vector2.MoveTowards(
            transform.position,
            m_points[m_currentPointIndex].position,
            m_speed * Time.deltaTime
        );
    }
    #endregion
}
