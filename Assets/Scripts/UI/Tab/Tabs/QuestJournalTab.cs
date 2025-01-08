using PixelCrushers.QuestMachine;
using UnityEngine;

/// <summary>
/// Manages the quest journal tab in the UI, handling the display and updates of quest information.
/// </summary>
public class QuestJournalTab : Tab
{
    [Tooltip("Reference to the Unity UI Quest Journal component")]
    [SerializeField] private UnityUIQuestJournalUI unityUIQuestJournalUI;

    /// <summary>
    /// Sets up the default quest journal UI on initialization.
    /// </summary>
    private void Awake()
    {
        QuestMachine.defaultQuestJournalUI = unityUIQuestJournalUI;
    }

    /// <summary>
    /// Called when the tab becomes active. Updates the quest journal display.
    /// </summary>
    public override void OnActive()
    {
        base.OnActive();

        if (QuestMachine.defaultQuestJournalUI != null)
            unityUIQuestJournalUI.Redraw();
    }
}
