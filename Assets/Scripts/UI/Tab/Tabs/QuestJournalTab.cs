using PixelCrushers.QuestMachine;
using UnityEngine;
using UnityCore.AudioManager;

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
    /// Resets the quest UI audio state when the tab is opened.
    /// </summary>
    private void OnEnable()
    {
        // Reset quest UI audio state when tab is opened
        QuestUIAudioState.Reset();
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
