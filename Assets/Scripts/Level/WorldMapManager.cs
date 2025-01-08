using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the world map system, handling room transitions, state persistence, and room visibility.
/// This class is responsible for tracking active rooms, handling room changes, and managing the map state.
/// </summary>
public class WorldMapManager : MonoBehaviour
{
    #region Room Management

    /// <summary>
    /// List of all rooms in the world map. Each room represents a distinct area in the game.
    /// </summary>
    [Header("Room Management")]
    [Tooltip("List of all rooms in the world map")]
    public List<Room> m_rooms = new List<Room>();

    /// <summary>
    /// Currently active room where the player is located.
    /// </summary>
    [Tooltip("Currently active room")]
    public Room m_activeRoom;

    /// <summary>
    /// ID of the current level/room. Used to track player location.
    /// </summary>
    [Tooltip("Current level/room ID")]
    public int current_levelID = 0;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the world map manager by setting up event subscriptions and finding all room components.
    /// Called when the world map is first created.
    /// </summary>
    public void Initialize()
    {
        // Subscribe to game events
        SubscribeToEvents();

        // Find and register all room components
        FindAndRegisterRooms();
    }

    /// <summary>
    /// Subscribes to all necessary game events for room management.
    /// </summary>
    private void SubscribeToEvents()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onRoomChanged += OnRoomChanged;
            GameEvents.Instance.onGameSaving += SaveRoomStatus;
            GameEvents.Instance.onPlayerDied += SaveRoomStatus;
            GameEvents.Instance.onHiddenRoomRevealed += HiddenRoomRevealed;
        }
        else
        {
            Debug.LogError("[WorldMapManager] GameEvents.Instance is null during initialization!");
        }
    }

    /// <summary>
    /// Finds all Room components in child objects and registers them.
    /// </summary>
    private void FindAndRegisterRooms()
    {
        m_rooms.Clear(); // Clear existing rooms to prevent duplicates
        
        foreach (Transform child in transform)
        {
            Room roomComponent = child.GetComponent<Room>();
            if (roomComponent != null)
            {
                m_rooms.Add(roomComponent);
            }
        }

        if (m_rooms.Count == 0)
        {
            Debug.LogWarning("[WorldMapManager] No rooms found in children!");
        }
    }

    #endregion

    #region Room State Management

    /// <summary>
    /// Saves the active state of all rooms to the dialogue system for persistence.
    /// Called when the game is saving or when the player dies.
    /// </summary>
    private void SaveRoomStatus()
    {
        foreach (var room in m_rooms)
        {
            if (room != null && room.gameObject.activeSelf)
            {
                DialogueLua.SetVariable($"Room.{room.m_roomID}", true);
            }
        }
    }

    #endregion

    #region Event Handling

    /// <summary>
    /// Cleans up by unsubscribing from all events.
    /// Called when the world map is being destroyed.
    /// </summary>
    public void Deinitialize()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onGameSaving -= SaveRoomStatus;
            GameEvents.Instance.onRoomChanged -= OnRoomChanged;
            GameEvents.Instance.onPlayerDied -= SaveRoomStatus;
            GameEvents.Instance.onHiddenRoomRevealed -= HiddenRoomRevealed;
        }
    }

    /// <summary>
    /// Reveals hidden areas in the active room.
    /// Called when a hidden room is discovered.
    /// </summary>
    private void HiddenRoomRevealed()
    {
        if (m_activeRoom != null)
        {
            m_activeRoom.RevealHidden();
        }
    }

    /// <summary>
    /// Handles room transitions when the player enters a new room.
    /// Deactivates the previous room and activates the new one.
    /// </summary>
    /// <param name="levelID">ID of the new room/level to transition to</param>
    private void OnRoomChanged(int levelID)
    {
        current_levelID = levelID;

        foreach (var room in m_rooms)
        {
            if (room != null && room.m_roomID == levelID)
            {
                // Deactivate previous room if it exists and is different
                if (m_activeRoom != null && m_activeRoom != room)
                {
                    m_activeRoom.SetRoomInActive();
                }

                // Activate new room
                m_activeRoom = room;
                m_activeRoom.SetRoomActive();
                return;
            }
        }

        Debug.LogWarning($"[WorldMapManager] Could not find room with ID {levelID}!");
    }

    #endregion
}
