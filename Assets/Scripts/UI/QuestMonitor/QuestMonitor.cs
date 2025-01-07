using UnityEngine;
using PixelCrushers;
using PixelCrushers.QuestMachine;
using TMPro;

/// <summary>
/// Monitors and displays quest state changes with animated notifications.
/// Handles quest updates, new quests, and quest completions with different animations.
/// </summary>
public class QuestMonitor : MonoBehaviour, IMessageHandler
{
    #region Serialized Fields
    /// <summary>
    /// Text component for displaying the quest name.
    /// </summary>
    [SerializeField] private TextMeshProUGUI questName;

    /// <summary>
    /// Animator component for quest notification animations.
    /// </summary>
    [SerializeField] private Animator questImageAnimator;
    #endregion

    #region Private Fields
    /// <summary>
    /// Defines the different types of quest animations available.
    /// </summary>
    private enum QuestAnimationMode
    {
        New,
        Update,
        Completed
    }

    private QuestAnimationMode questAnimationMode;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Validates required components on startup.
    /// </summary>
    private void Awake()
    {
        ValidateComponents();
    }

    /// <summary>
    /// Subscribes to quest state change messages.
    /// </summary>
    private void OnEnable()
    {
        MessageSystem.AddListener(this, QuestMachineMessages.QuestStateChangedMessage, string.Empty);
    }

    /// <summary>
    /// Unsubscribes from quest state change messages.
    /// </summary>
    private void OnDisable()
    {
        MessageSystem.RemoveListener(this);
    }
    #endregion

    #region Component Validation
    /// <summary>
    /// Validates that all required components are properly assigned.
    /// </summary>
    private void ValidateComponents()
    {
        if (questName == null)
        {
            Debug.LogError($"[QuestMonitor] Missing questName Text component on {gameObject.name}");
            enabled = false;
            return;
        }

        if (questImageAnimator == null)
        {
            Debug.LogError($"[QuestMonitor] Missing questImageAnimator component on {gameObject.name}");
            enabled = false;
            return;
        }
    }
    #endregion

    #region Message Handling
    /// <summary>
    /// Handles incoming quest state change messages.
    /// </summary>
    /// <param name="messageArgs">Message arguments containing quest state information</param>
    public void OnMessage(MessageArgs messageArgs)
    {
        if (messageArgs.values == null || messageArgs.values.Length < 2)
        {
            Debug.LogWarning("[QuestMonitor] Received invalid message args");
            return;
        }

        string questID = messageArgs.parameter;
        if (string.IsNullOrEmpty(questID))
        {
            Debug.LogWarning("[QuestMonitor] Received empty quest ID");
            return;
        }

        questName.text = questID;

        if (messageArgs.values[0] == null)
        {
            HandleQuestStateChange((QuestState)messageArgs.values[1]);
        }
        else
        {
            HandleQuestNodeStateChange((StringField)messageArgs.values[0], (QuestNodeState)messageArgs.values[1]);
        }

        PlayAnimation();
    }

    /// <summary>
    /// Handles changes in the main quest state.
    /// </summary>
    /// <param name="state">New quest state</param>
    private void HandleQuestStateChange(QuestState state)
    {
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

    /// <summary>
    /// Handles changes in quest node states.
    /// </summary>
    /// <param name="nodeId">Node identifier</param>
    /// <param name="state">New node state</param>
    private void HandleQuestNodeStateChange(StringField nodeId, QuestNodeState state)
    {
        if (nodeId.value.Contains("Condition") && state == QuestNodeState.True)
        {
            questAnimationMode = QuestAnimationMode.Update;
        }
    }
    #endregion

    #region Animation
    /// <summary>
    /// Plays the animation corresponding to the current quest animation mode.
    /// </summary>
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
    #endregion
}
