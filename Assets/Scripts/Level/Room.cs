using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    [SerializeField] public int m_roomID = 0;
    [SerializeField] private GameObject m_playerIcon;

    public void SetRoomActive()
    {
        m_playerIcon.SetActive(true);
    }
    public void SetRoomInActive()
    {
        m_playerIcon.SetActive(false);
    }
}
