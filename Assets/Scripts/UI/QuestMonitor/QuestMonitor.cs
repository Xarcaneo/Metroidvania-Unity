using UnityEngine;
using PixelCrushers;
using PixelCrushers.QuestMachine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class QuestMonitor : MonoBehaviour, IMessageHandler
{
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private Animator questImageAnimator;

    [SerializeField] private float timeToReturnToIdle = 25.0f;

    private enum QuestAnimationMode { New, Update, Completed }
    private QuestAnimationMode questAnimationMode;

    private void OnEnable()
    {
        // Listen for Quest State Changed messages
        MessageSystem.AddListener(this, QuestMachineMessages.QuestStateChangedMessage, string.Empty);
    }

    private void OnDisable()
    {
        // Stop listening
        MessageSystem.RemoveListener(this);
    }

    public void OnMessage(MessageArgs messageArgs)
    {
        string questID = messageArgs.parameter;
        if (messageArgs.values[0] == null)
        {
            QuestState state = (QuestState)messageArgs.values[1];
            questName.text = questID;

            switch (state)
            {
                case QuestState.Active:
                    questAnimationMode = QuestAnimationMode.New;
                    break;
                case QuestState.Successful:
                    questAnimationMode = QuestAnimationMode.Completed;
                    break;
            }
        }
        else
        {
            StringField nodeID = (StringField)messageArgs.values[0];
            QuestNodeState state = (QuestNodeState)messageArgs.values[1];

            if (nodeID.value.Contains("Condition") && state == QuestNodeState.True)
            {
                questAnimationMode = QuestAnimationMode.Update;
            }
        }
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        switch (questAnimationMode)
        {
            case QuestAnimationMode.New:
                questImageAnimator.Play("NewQuest");
                break;
            case QuestAnimationMode.Update:
                questImageAnimator.Play("UpdateQuest");
                break;
            case QuestAnimationMode.Completed:
                questImageAnimator.Play("CompletedQuest");
                break;
        }
    }
}
