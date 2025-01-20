using PixelCrushers.DialogueSystem;
using UnityEngine;

/// <summary>
/// A derived event class that starts a dialogue conversation using the Pixel Crushers Dialogue System
/// when the trigger conditions are met.
/// </summary>
public class DialogueEvent : BaseAreaEvent
{
    /// <summary>
    /// The name of the conversation to start when this event is triggered.
    /// </summary>
    [SerializeField]
    private string m_dialogueID;

    /// <summary>
    /// Executes the dialogue event logic by starting a conversation with the specified name.
    /// </summary>
    /// <param name="collider">The collider that triggered the event.</param>
    protected override void Execute(Collider2D collider)
    {
        if (string.IsNullOrEmpty(m_dialogueID))
        {
            Debug.LogWarning($"[DialogueEvent] Conversation name is empty!");
            return;
        }

        // Start the conversation using the Dialogue System
        DialogueManager.StartConversation(m_dialogueID);
    }
}
