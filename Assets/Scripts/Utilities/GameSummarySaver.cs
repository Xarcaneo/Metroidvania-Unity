using PixelCrushers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSummarySaver : Saver
{
    [Serializable]
    public class Data
    {
        public string sceneName;
        //public string totalPlayTime;
    }

    public override string RecordData()
    {
        var data = new Data();
        data.sceneName = SceneManager.GetActiveScene().name;
        // ... etc. for everything in Data ...
        return SaveSystem.Serialize(data);
    }

    public override void ApplyData(string s)
    {
        // Do nothing. We only record the data for the save/load menu.
        // There's no need to re-apply it when loading a saved game.
    }
}