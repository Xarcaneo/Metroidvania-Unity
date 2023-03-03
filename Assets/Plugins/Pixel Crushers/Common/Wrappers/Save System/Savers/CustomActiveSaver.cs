using PixelCrushers.Wrappers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomActiveSaver : ActiveSaver
{
    public static bool IsLoadingSavedGame = false;

    public override void ApplyData(string s)
    {
        if (!IsLoadingSavedGame) base.ApplyData(s);
    }
}