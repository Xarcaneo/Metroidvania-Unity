using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    [SerializeField] public int m_roomID = 0;
    [SerializeField] private GameObject m_playerIcon;
    [SerializeField] private bool isActiveByDefault = false;
    [SerializeField] private GameObject wallsImage;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        var roomState = DialogueLua.GetVariable("Room." + m_roomID).asBool;

        if (roomState || isActiveByDefault) gameObject.SetActive(true);
        else gameObject.SetActive(false);

        WorldMapManager worldMapManager = FindObjectOfType<WorldMapManager>();
        if (worldMapManager != null && !worldMapManager.m_rooms.Contains(this))
        {
            Debug.LogError("The current Room " + m_roomID + " component is not in the m_rooms list of the WorldMapManager script.");
        }
    }

    public void SetRoomActive()
    {
        if(m_playerIcon) 
            m_playerIcon.SetActive(true);

        ShowRoom();
    }
    public void SetRoomInActive()
    {
        m_playerIcon.SetActive(false);
    }

    private void ShowRoom()
    {
        gameObject.SetActive(true);
    }

    public void ReplaceWallsImage() => wallsImage.GetComponent<Image>().enabled = false;
}
