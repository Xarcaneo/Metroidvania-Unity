using UnityEngine;
using UnityCore.AudioManager;

/// <summary>
/// A simple component that plays a selected audio event.
/// Can be triggered via code or events.
/// </summary>
public class SimpleAudioPlayer : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("The audio event to play")]
    [SerializeField] private AudioEventId m_AudioEvent;

    [Tooltip("Whether to play the sound automatically when this component is enabled")]
    [SerializeField] private bool m_PlayOnEnable = false;

    private void OnEnable()
    {
        if (m_PlayOnEnable)
        {
            Play();
        }
    }

    /// <summary>
    /// Plays the selected audio event.
    /// Can be called from other scripts or via Unity Events in the inspector.
    /// </summary>
    public void Play()
    {
        AudioManager.instance.PlaySound(m_AudioEvent);
    }

    /// <summary>
    /// Changes the audio event and optionally plays it immediately.
    /// </summary>
    /// <param name="audioEvent">The new audio event to use</param>
    /// <param name="playImmediately">Whether to play the sound immediately after changing</param>
    public void SetAudioEvent(AudioEventId audioEvent, bool playImmediately = false)
    {
        m_AudioEvent = audioEvent;
        
        if (playImmediately)
        {
            Play();
        }
    }
}
