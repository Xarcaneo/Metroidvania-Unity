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
    /// Cached reference to GameEvents instance.
    /// </summary>
    private GameEvents m_gameEvents;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        m_gameEvents = GameEvents.Instance;
        if (m_gameEvents == null)
        {
            Debug.LogError($"[{gameObject.name}] GameEvents instance is null!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only notify if it's the player entering
        if (collision.CompareTag("Player"))
        {
            if (m_gameEvents != null)
            {
                m_gameEvents.RoomChanged(roomNumber);
            }

            // Notify the AreaManager of the active area
            AreaManager.Instance?.SetActiveArea(this);
        }
    }
    #endregion
}
