using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapManager : MonoBehaviour
{
    public List<Room> m_rooms = new List<Room>();
    public Room m_activeRoom;
    public int current_levelID = 0;

    public void Initialize()
    {
        GameEvents.Instance.onRoomChanged += OnRoomChanged;
        GameEvents.Instance.onGameSaving += OnGameSaving;
        GameEvents.Instance.onHiddenRoomRevealed += HiddenRoomRevealed;

        foreach (Transform child in transform)
        {
            Room roomComponent = child.GetComponent<Room>();
            if (roomComponent != null)
            {
                m_rooms.Add(roomComponent);
            }
        }
    }
    
    private void OnGameSaving()
    {
        foreach (var room in m_rooms)
        {
            if(room.gameObject.activeSelf)
                DialogueLua.SetVariable("Room." + room.m_roomID, true);
        }
    }

    public void Deinitialize()
    {
        GameEvents.Instance.onGameSaving -= OnGameSaving;
        GameEvents.Instance.onRoomChanged -= OnRoomChanged;
        GameEvents.Instance.onHiddenRoomRevealed -= HiddenRoomRevealed;
    }

    private void HiddenRoomRevealed() => m_activeRoom.RevealHidden();

    private void OnRoomChanged(int levelID)
    {
        current_levelID = levelID;

        foreach (var room in m_rooms)
        {
            if (room.m_roomID == levelID)
            {
                if (m_activeRoom) m_activeRoom.SetRoomInActive();

                m_activeRoom = room;
                m_activeRoom.SetRoomActive();
                return;
            }
        }
    }
}
