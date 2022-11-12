using PixelCrushers.QuestMachine;
using UnityEngine;

public class QuestJournalTab : MonoBehaviour
{
    [SerializeField] private UnityUIQuestJournalUI unityUIQuestJournalUI;

    private void Start() => QuestMachine.defaultQuestJournalUI = unityUIQuestJournalUI;
    private void OnEnable() => unityUIQuestJournalUI.Repaint();
}
