using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a room in the world map, managing its visibility, state, and appearance.
/// Each room can have a player icon, hidden areas, and different visual states.
/// </summary>
public class Room : MonoBehaviour
{
    #region Room Properties

    /// <summary>
    /// Unique identifier for this room. Used to track room state and transitions.
    /// </summary>
    [Header("Room Settings")]
    [Tooltip("Unique identifier for this room")]
    [SerializeField] public int m_roomID = 0;

    /// <summary>
    /// Icon that shows the player's current position when in this room.
    /// </summary>
    [Tooltip("Icon showing player's position in this room")]
    [SerializeField] private GameObject m_playerIcon;

    /// <summary>
    /// If true, room will be visible on the map even if not yet visited.
    /// </summary>
    [Tooltip("Should this room be visible before being visited?")]
    [SerializeField] private bool isActiveByDefault = false;

    #endregion

    #region Visual Elements

    /// <summary>
    /// Image component showing the room's walls.
    /// </summary>
    [Header("Visual Elements")]
    [Tooltip("Image showing room walls")]
    [SerializeField] private Image wallsImage;

    /// <summary>
    /// Sprite to show when hidden areas are revealed.
    /// </summary>
    [Tooltip("Sprite for revealed hidden areas")]
    [SerializeField] private Sprite hiddenWallsImage;

    #endregion

    #region State Tracking

    /// <summary>
    /// Whether the player has visited this room.
    /// </summary>
    private bool hasVisited = false;

    /// <summary>
    /// Whether hidden areas in this room have been revealed.
    /// </summary>
    private bool hiddenRevealed = false;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes room state from saved data and updates appearance.
    /// Waits for end of frame to ensure all components are ready.
    /// </summary>
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        // Load saved state
        LoadRoomState();
        
        // Set initial visibility
        gameObject.SetActive(hasVisited || isActiveByDefault);
        
        // Update visual appearance
        UpdateRoomAppearance();
    }

    /// <summary>
    /// Loads the room's saved state from the dialogue system.
    /// </summary>
    private void LoadRoomState()
    {
        hasVisited = DialogueLua.GetVariable($"Room.{m_roomID}").asBool;
        hiddenRevealed = DialogueLua.GetVariable($"Room.Hidden.{m_roomID}").asBool;
    }

    #endregion

    #region Room State Management

    /// <summary>
    /// Activates this room and shows the player icon.
    /// Called when the player enters this room.
    /// </summary>
    public void SetRoomActive()
    {
        if (m_playerIcon != null)
        {
            m_playerIcon.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"[Room {m_roomID}] Player icon is missing!", this);
        }

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivates the player icon when leaving this room.
    /// The room itself remains visible if previously visited.
    /// </summary>
    public void SetRoomInActive()
    {
        if (m_playerIcon != null)
        {
            m_playerIcon.SetActive(false);
        }
    }

    /// <summary>
    /// Reveals hidden areas in the room and updates its appearance.
    /// Also saves the revealed state to persistence.
    /// </summary>
    public void RevealHidden()
    {
        hiddenRevealed = true;
        DialogueLua.SetVariable($"Room.Hidden.{m_roomID}", true);
        UpdateRoomAppearance();
    }

    #endregion

    #region Visual Updates

    /// <summary>
    /// Updates the room's visual appearance based on its current state.
    /// Changes the walls sprite if hidden areas are revealed.
    /// </summary>
    private void UpdateRoomAppearance()
    {
        if (wallsImage != null && hiddenWallsImage != null && hiddenRevealed)
        {
            wallsImage.sprite = hiddenWallsImage;
        }
    }

    #endregion
}
