using UnityEngine;

/// <summary>
/// Static class to manage dialogue audio state
/// </summary>
public static class DialogueAudioState
{
    public static bool IsFirstSelection { get; private set; } = true;

    public static void Reset()
    {
        IsFirstSelection = true;
    }

    public static void MarkSelectionMade()
    {
        IsFirstSelection = false;
    }
}
