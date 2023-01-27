using Audio;
using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] int ChapterNumber = 0;

    private void OnEnable()
    {
        AudioManager.Instance.Jukebox.SetAudioCollection(ChapterNumber);
        AudioManager.Instance.Jukebox.gameObject.SetActive(true);
    }

    private void OnApplicationQuit()
    {
        if (Player.Instance.GetComponent<PlayerPositionSaver>().m_lastCheckpointPosition != new Vector3(0, 0, 0))
            SaveSystem.SaveToSlot(GameManager.Instance.currentSaveSlot);
    }
}
