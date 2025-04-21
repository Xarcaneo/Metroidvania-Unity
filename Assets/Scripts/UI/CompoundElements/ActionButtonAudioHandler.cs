using UnityEngine;
using UnityEngine.EventSystems;
using Opsive.UltimateInventorySystem.UI.CompoundElements;
using UnityCore.AudioManager;
using System.Collections;

/// <summary>
/// Static class to manage action button audio state
/// </summary>
public static class ActionButtonAudioState
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
/// Handles audio feedback for ActionButton interactions.
/// Attach this component to the same GameObject as ActionButton.
/// </summary>
[RequireComponent(typeof(ActionButton))]
public class ActionButtonAudioHandler : MonoBehaviour
{
    private ActionButton m_ActionButton;
    private float lastSelectionTime;

    [Header("Audio Settings")]
    [Tooltip("Sound played when selecting the button")]
    [SerializeField] private AudioEventId m_SelectSound = AudioEventId.UI_Button_Focus;
    [Tooltip("Sound played when clicking/submitting the button")]
    [SerializeField] private AudioEventId m_ClickSound = AudioEventId.UI_Button_Press;

    private void Awake()
    {
        m_ActionButton = GetComponent<ActionButton>();
        if (m_ActionButton == null)
        {
            Debug.LogError($"[ActionButtonAudioHandler] No ActionButton found on {gameObject.name}", this);
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        if (m_ActionButton != null)
        {
            m_ActionButton.OnSelectE += OnButtonSelect;
            m_ActionButton.OnSubmitE += OnButtonSubmit;
        }

        // Record the time when this button was enabled
        lastSelectionTime = Time.unscaledTime;
        
        // Start checking if this was a real selection after a frame
        StartCoroutine(CheckIfRealSelection());
    }

    private void OnDisable()
    {
        if (m_ActionButton != null)
        {
            m_ActionButton.OnSelectE -= OnButtonSelect;
            m_ActionButton.OnSubmitE -= OnButtonSubmit;
        }
    }

    private void OnButtonSelect()
    {
        // If enough time has passed since enabling, this is a real user selection
        if (Time.unscaledTime - lastSelectionTime > 0.1f)
        {
            AudioManager.instance.PlayUISound(m_SelectSound);
        }
        lastSelectionTime = Time.unscaledTime;
    }

    private void OnButtonSubmit()
    {
        AudioManager.instance.PlayUISound(m_ClickSound);
    }

    private IEnumerator CheckIfRealSelection()
    {
        yield return null; // Wait one frame

        // If this button is already selected right after enabling,
        // it was an automatic selection, so update the time
        if (EventSystem.current != null && 
            EventSystem.current.currentSelectedGameObject == gameObject)
        {
            lastSelectionTime = Time.unscaledTime;
        }
    }
}
