using UnityEngine;
using UnityEngine.EventSystems;
using PixelCrushers.DialogueSystem;
using UnityCore.AudioManager;

/// <summary>
/// Handles audio feedback for StandardUIResponseButton interactions.
/// Attach this component to the same GameObject as StandardUIResponseButton.
/// </summary>
[RequireComponent(typeof(StandardUIResponseButton))]
public class ResponseButtonAudioHandler : MonoBehaviour, ISelectHandler
{
    [Header("Audio Settings")]
    [Tooltip("Sound played when selecting the button")]
    [SerializeField] private AudioEventId m_SelectSound = AudioEventId.UI_Button_Focus;

    private StandardUIResponseButton m_ResponseButton;

    private void Awake()
    {
        m_ResponseButton = GetComponent<StandardUIResponseButton>();
    }

    private void OnEnable()
    {
        // Reset audio state when dialogue UI is enabled
        DialogueAudioState.Reset();
    }

    /// <summary>
    /// Called by Unity's event system when this button is selected.
    /// </summary>
    public void OnSelect(BaseEventData eventData)
    {
        // Skip playing sound for the first selection when dialogue UI opens
        if (!DialogueAudioState.IsFirstSelection)
        {
            AudioManager.instance.PlaySound(m_SelectSound);
        }
        
        DialogueAudioState.MarkSelectionMade();
    }
}
