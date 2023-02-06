using Audio;
using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] int ChapterNumber = 0;
    [SerializeField] int m_roomID = 0;

    [SerializeField] Player m_playerPref;

    private void OnEnable()
    {
        AudioManager.Instance.Jukebox.SetAudioCollection(ChapterNumber);
        AudioManager.Instance.Jukebox.gameObject.SetActive(true);
        GameEvents.Instance.LevelLoaded(m_roomID);

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        var savedGameData = SaveSystem.storer.RetrieveSavedGameData(GameManager.Instance.currentSaveSlot);
        var s = savedGameData.GetData("playerPositionKey");
        if (!string.IsNullOrEmpty(s))
        {
            var positionData = SaveSystem.Deserialize<PositionSaver.PositionData>(s);
            Instantiate(m_playerPref, positionData.position, Quaternion.identity);
        }
        else
        {
            var position = GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;
            Instantiate(m_playerPref, position, Quaternion.identity);
        }    
    }
}
