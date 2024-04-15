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

    [SerializeField] private float timeToReturnToIdle = 2.0f;

    private bool isLoadingScene;

    enum QuestAnimationMode { New, Update, Completed};
    private QuestAnimationMode questAnimationMode;

    private void OnEnable()
    {
        // Listen for Quest State Changed messages:
        MessageSystem.AddListener(this, QuestMachineMessages.QuestStateChangedMessage, string.Empty);
        SaveSystem.saveDataApplied += OnSaveDataApplied;
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
    }

    private void OnDisable()
    {
        // Stop listening:
        MessageSystem.RemoveListener(this);
        SaveSystem.saveDataApplied -= OnSaveDataApplied;
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to avoid memory leaks
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => isLoadingScene = true; // Set isLoadingScene to true when a new scene is loaded
    private void OnSaveDataApplied() => isLoadingScene = false;
    
    public void OnMessage(MessageArgs messageArgs)
    {
        if (isLoadingScene) return;
        
        // We received a quest state changed message. Log the state.
        // Parameter: Quest ID. 
        // Argument 0: [StringField] Quest node ID, or null for main quest state.
        // Argument 1: [QuestState] / [QuestNodeState] New state.

        string questID = messageArgs.parameter;
        if (messageArgs.values[0] == null) // (null means this message is for main quest state)
        {
            QuestState state = (QuestState)messageArgs.values[1];
            questName.text = questID;

            if (state.ToString() == "Active")
            {
                questImageAnimator.Play("Intro");
                questAnimationMode = QuestAnimationMode.New;
            }
            else if (state.ToString() == "Successful")
            {
                questImageAnimator.Play("Intro");
                questAnimationMode = QuestAnimationMode.Completed;
            }

            //Debug.Log($"Quest '{questID}' changed to state {state}");
        }
        else
        {
            StringField nodeID = (StringField)messageArgs.values[0];
            QuestNodeState state = (QuestNodeState)messageArgs.values[1];

            // Check if the node ID contains the string "Condition"
            if (nodeID.value.Contains("Condition"))
            {
                if (state.ToString() == "True")
                {
                    questImageAnimator.Play("Intro");
                    questAnimationMode = QuestAnimationMode.Update;
                }
                //Debug.Log($"Quest '{questID}' node '{nodeID}' changed to state {state}");
            }
        }   
    }

    IEnumerator ReturnToIdleAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        questImageAnimator.Play("Idle");
    }

    public void OnIntroAnimationFinished()
    {
        switch (questAnimationMode)
        {
            case QuestAnimationMode.New:
                questImageAnimator.Play("NewQuest");
                break;

            case QuestAnimationMode.Update:
                questImageAnimator.Play("NewQuest");
                break;

            case QuestAnimationMode.Completed:
                questImageAnimator.Play("NewQuest");
                break;

        }
        StartCoroutine(ReturnToIdleAfterTime(timeToReturnToIdle));
    }
}
