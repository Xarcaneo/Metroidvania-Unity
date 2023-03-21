using System.Collections.Generic;
using PixelCrushers;

public class CustomMultiActiveSaverManager : Saver
{
    public static bool IsLoadingSavedGame { get; set; } = false;

    public override void Start()
    {
        base.Start();
        SaveSystem.loadStarted += () => IsLoadingSavedGame = true;
        SaveSystem.saveDataApplied += () => IsLoadingSavedGame = false;
    }

    public override string RecordData()
    {
        return string.Empty;
    }

    public override void ApplyData(string s)
    {
        if (IsLoadingSavedGame)
        {
            var deleteList = new List<string>();
            foreach (var key in SaveSystem.currentSavedGameData.Dict.Keys)
            {
                if (key.StartsWith("OnlyOnLoad_")) deleteList.Add(key);
            }
            deleteList.ForEach(key => SaveSystem.currentSavedGameData.Dict.Remove(key));
        }
    }

}
