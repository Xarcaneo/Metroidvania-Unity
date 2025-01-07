using PixelCrushers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles saving and recording summary data for a game session, primarily for use with a save/load menu.
/// Inherits from PixelCrushers' Saver class to integrate with the save system.
/// </summary>
public class GameSummarySaver : Saver
{
    /// <summary>
    /// Data structure to hold scene-related summary information.
    /// </summary>
    [Serializable]
    public class Data
    {
        /// <summary>
        /// The name of the currently active scene.
        /// </summary>
        public string sceneName;

        // Future extension: Uncomment the following if you want to track total playtime.
        // public string totalPlayTime;
    }

    /// <summary>
    /// Records the current game summary data to be saved.
    /// </summary>
    /// <returns>A serialized string containing the scene name.</returns>
    public override string RecordData()
    {
        var data = new Data();
        data.sceneName = SceneManager.GetActiveScene().name;
        // Additional fields from Data can be added here if needed.
        return SaveSystem.Serialize(data);
    }

    /// <summary>
    /// Applies the saved game summary data when loading a saved game.
    /// This implementation does nothing as the data is only used for the save/load menu.
    /// </summary>
    /// <param name="s">Serialized data string.</param>
    public override void ApplyData(string s)
    {
        // Do nothing. We only record the data for the save/load menu.
        // There's no need to re-apply it when loading a saved game.
    }
}
