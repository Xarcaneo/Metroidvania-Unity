using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapManager : MonoBehaviour
{
    public Room[] m_rooms;
    public Room m_activeRoom;
    public int current_levelID = 0;

    private void Awake()
    {
        GameEvents.Instance.onLevelLoaded += OnLevelLoaded;

        m_rooms = FindObjectsOfType<Room>();
    }
    private void OnDestroy() => GameEvents.Instance.onLevelLoaded -= OnLevelLoaded;

    private void OnLevelLoaded(int levelID)
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
