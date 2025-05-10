using UnityEngine;
using UnityEngine.EventSystems;
using UnityCore.AudioManager;
using PixelCrushers.QuestMachine;

/// <summary>
/// Static class to manage quest UI audio state
/// </summary>
public static class QuestUIAudioState
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

/// <summary>
/// Handles audio feedback for UnityUIButtonTemplate interactions.
/// Attach this component to the same GameObject as UnityUIButtonTemplate.
/// </summary>
[RequireComponent(typeof(UnityUIButtonTemplate))]
public class QuestButtonAudioHandler : MonoBehaviour, ISelectHandler
{
    [Header("Audio Settings")]
    [Tooltip("Sound played when selecting the button")]
    [SerializeField] private AudioEventId m_SelectSound = AudioEventId.UI_Button_Focus;

    private UnityUIButtonTemplate m_ButtonTemplate;

    private void Awake()
    {
        m_ButtonTemplate = GetComponent<UnityUIButtonTemplate>();
        if (m_ButtonTemplate == null)
        {
            Debug.LogError($"[QuestButtonAudioHandler] No UnityUIButtonTemplate found on {gameObject.name}", this);
            enabled = false;
            return;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Play select sound when button is selected (except for first selection)
        if (!QuestUIAudioState.IsFirstSelection)
        {
            AudioManager.instance.PlaySound(m_SelectSound);
        }
        QuestUIAudioState.MarkSelectionMade();
    }
}
