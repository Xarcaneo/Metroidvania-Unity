using PixelCrushers.QuestMachine;
using UnityEngine;

public class QuestJournalTab : MonoBehaviour
{
    [SerializeField] private UnityUIQuestJournalUI unityUIQuestJournalUI;

    private void OnEnable()
    {
        if (QuestMachine.defaultQuestJournalUI == null) QuestMachine.defaultQuestJournalUI = unityUIQuestJournalUI;

        Player.Instance.questJournal.ShowJournalUI();
    }
}
