using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    [SerializeField] public int m_roomID = 0;
    [SerializeField] private GameObject m_playerIcon;
    [SerializeField] private bool isActiveByDefault = false;
    [SerializeField] private Image wallsImage;
    [SerializeField] private Sprite hiddenWallsImage;

    private bool hasVisited = false;
    private bool hiddenRevealed = false;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        hasVisited = DialogueLua.GetVariable("Room." + m_roomID).asBool;
        hiddenRevealed = DialogueLua.GetVariable("Room.Hidden." + m_roomID).asBool;

        gameObject.SetActive(hasVisited || isActiveByDefault);
        UpdateRoomAppearance();
    }

    public void SetRoomActive()
    {
        if (m_playerIcon)
            m_playerIcon.SetActive(true);
        gameObject.SetActive(true);
    }

    public void SetRoomInActive()
    {
        m_playerIcon.SetActive(false);
    }

    public void RevealHidden()
    {
        hiddenRevealed = true;
        DialogueLua.SetVariable("Room.Hidden." + m_roomID, true);
        UpdateRoomAppearance();
    }

    private void UpdateRoomAppearance()
    {
        if (hiddenRevealed)
        {
            wallsImage.sprite = hiddenWallsImage;
        }
    }
}
