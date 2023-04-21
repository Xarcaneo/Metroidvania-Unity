using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapManager : MonoBehaviour
{
    public List<Room> m_rooms;
    public Room m_activeRoom;
    public int current_levelID = 0;

    public void Initialize()
    {
        GameEvents.Instance.onRoomChanged += OnRoomChanged;
    }
    private void OnDestroy()
    {
        GameEvents.Instance.onRoomChanged -= OnRoomChanged;
    }

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
