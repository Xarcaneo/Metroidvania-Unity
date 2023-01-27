using PixelCrushers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int currentSaveSlot;
    public bool shouldFlipPlayer = false;

    #region Instance variables
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; }
    #endregion

    #region Initialize functions
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start() => GameEvents.Instance.onPlayerSpawned += RetrieveLastCheckpoint;

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }

        GameEvents.Instance.onPlayerSpawned -= RetrieveLastCheckpoint;
    }

    #endregion

    #region Other functions
    public void RetrieveLastCheckpoint()
    {
        if (SaveSystem.HasSavedGameInSlot(currentSaveSlot))
        {
            var savedGameData = SaveSystem.storer.RetrieveSavedGameData(currentSaveSlot);
            var s = savedGameData.GetData("playerPositionKey");
            if (!string.IsNullOrEmpty(s))
            {
                var positionData = SaveSystem.Deserialize<PositionSaver.PositionData>(s);
                Player.Instance.GetComponent<PlayerPositionSaver>().m_lastCheckpointPosition = positionData.position;
            }
        }
    }
    #endregion
}
