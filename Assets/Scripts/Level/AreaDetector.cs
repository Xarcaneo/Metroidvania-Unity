using UnityEngine;

/// <summary>
/// Detects when a player enters a new area/room and notifies the game system.
/// Attach this to trigger colliders that define room boundaries.
/// </summary>
public class AreaDetector : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Unique identifier for this room/area")]
    public int roomNumber;
    #endregion

    #region Private Fields
    /// <summary>
    /// Cached reference to GameEvents instance
    /// </summary>
    private GameEvents m_gameEvents;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Validates component setup in the Unity Editor
    /// </summary>
    private void OnValidate()
    {
        if (roomNumber < 0)
        {
            Debug.LogWarning($"[{gameObject.name}] Room number should not be negative!");
        }
    }

    /// <summary>
    /// Initializes required components
    /// </summary>
    private void Awake()
    {
        m_gameEvents = GameEvents.Instance;
        if (m_gameEvents == null)
        {
            Debug.LogError($"[{gameObject.name}] GameEvents instance is null!");
        }
    }

    /// <summary>
    /// Detects when an object enters the area trigger
    /// </summary>
    /// <param name="collision">The collider that entered the trigger</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only notify if it's the player entering
        if (collision.CompareTag("Player"))
        {
            if (m_gameEvents != null)
            {
                m_gameEvents.RoomChanged(roomNumber);
            }
            else
            {
                Debug.LogError($"[{gameObject.name}] GameEvents instance is null!");
            }
        }
    }
    #endregion
}
