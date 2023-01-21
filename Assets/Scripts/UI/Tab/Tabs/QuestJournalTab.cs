using PixelCrushers.QuestMachine;
using UnityEngine;

public class QuestJournalTab : Tab
{
    [SerializeField] private UnityUIQuestJournalUI unityUIQuestJournalUI;

    private void Awake()
    {
        QuestMachine.defaultQuestJournalUI = unityUIQuestJournalUI;
    }

    public override void OnActive()
    {
        base.OnActive();

        if (QuestMachine.defaultQuestJournalUI != null)
            unityUIQuestJournalUI.Redraw();
    }

}
