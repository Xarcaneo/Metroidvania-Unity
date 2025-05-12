using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using FMOD;

/// <summary>
/// Manages background music playback during game sessions.
/// Plays tracks in random order and handles fading between tracks.
/// </summary>
public class BackgroundMusicManager : MonoBehaviour
{
    [Header("Music Settings")]
    [Tooltip("List of FMOD events to play in random order")]
    [SerializeField] private List<EventReference> musicEvents = new List<EventReference>();
    
    [Tooltip("Volume reduction in decibels for background music (-80 to 0)")]
    [Range(-40f, 0f)]
    [SerializeField] private float volumeDecibels = -6f;
    
    [Tooltip("Optional FMOD parameter name for volume control")]
    [SerializeField] private string volumeParameterName = "Volume";
    
    [Tooltip("Time to fade in/out when changing tracks (seconds)")]
    [SerializeField] private float fadeTime = 2.0f;
    
    [Tooltip("Delay between tracks (seconds)")]
    [SerializeField] private float delayBetweenTracks = 3.0f;
    
    [Tooltip("Should music loop continuously?")]
    [SerializeField] private bool loopMusic = true;
    
    // Tracking variables for playback
    private int currentTrackIndex = -1;
    private List<int> playedTrackIndices = new List<int>();
    private EventInstance currentEventInstance;
    private EventInstance nextEventInstance;
    private Coroutine fadeCoroutine;
    private Coroutine delayCoroutine;
    private Coroutine musicPlayerCoroutine;
    private bool isPlaying = false;
    
    // Tracking variables for playback
    
    private void Start()
    {
        
        // Subscribe to game events
        GameEvents.Instance.onNewSession += OnNewSession;
        GameEvents.Instance.onEndSession += OnEndSession;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from game events
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onNewSession -= OnNewSession;
            GameEvents.Instance.onEndSession -= OnEndSession;
        }
        
        // Clean up any FMOD instances
        StopAllMusic();
    }
    
    private void ReleaseEventInstances()
    {
        // Release current event instance if it exists
        if (currentEventInstance.isValid())
        {
            currentEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentEventInstance.release();
        }
        
        // Release next event instance if it exists
        if (nextEventInstance.isValid())
        {
            nextEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            nextEventInstance.release();
        }
    }
    
    /// <summary>
    /// Handles the start of a new game session.
    /// </summary>
    private void OnNewSession()
    {
        // Start background music when a new session begins
        StartBackgroundMusic();
    }
    
    /// <summary>
    /// Handles the end of a game session.
    /// </summary>
    private void OnEndSession()
    {
        // Stop all music when the session ends
        StopAllMusic();
    }
    

    
    /// <summary>
    /// Starts playing background music in random order.
    /// </summary>
    public void StartBackgroundMusic()
    {
        if (isPlaying || musicEvents.Count == 0)
            return;
            
        isPlaying = true;
        
        // Reset track history
        playedTrackIndices.Clear();
        currentTrackIndex = -1;
        
        // Start music player coroutine
        if (musicPlayerCoroutine != null)
            StopCoroutine(musicPlayerCoroutine);
            
        musicPlayerCoroutine = StartCoroutine(PlayMusicSequence());
    }
    
    /// <summary>
    /// Stops the background music with a fade out.
    /// </summary>
    public void StopBackgroundMusic()
    {
        if (!isPlaying)
            return;
            
        isPlaying = false;
        
        // Stop the music player coroutine
        if (musicPlayerCoroutine != null)
        {
            StopCoroutine(musicPlayerCoroutine);
            musicPlayerCoroutine = null;
        }
        
        // Fade out any currently playing music
        if (currentEventInstance.isValid())
        {
            StartCoroutine(FadeOutFMOD(currentEventInstance));
        }
        
        if (nextEventInstance.isValid())
        {
            StartCoroutine(FadeOutFMOD(nextEventInstance));
        }
    }
    
    /// <summary>
    /// Stops all currently playing music.
    /// </summary>
    private void StopAllMusic()
    {
        isPlaying = false;
        
        // Stop any running coroutines
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        
        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }
        
        // Stop and release FMOD instances
        if (currentEventInstance.isValid())
        {
            currentEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentEventInstance.release();
        }
        
        if (nextEventInstance.isValid())
        {
            nextEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            nextEventInstance.release();
        }
    }
    
    /// <summary>
    /// Coroutine that manages the sequence of music tracks.
    /// </summary>
    private IEnumerator PlayMusicSequence()
    {
        while (isPlaying)
        {
            // Select next track
            int nextTrackIndex = GetNextTrackIndex();
            
            if (nextTrackIndex >= 0 && nextTrackIndex < musicEvents.Count)
            {
                EventReference nextEvent = musicEvents[nextTrackIndex];
                currentTrackIndex = nextTrackIndex;
                
                // Play the track
                yield return PlayFMODEvent(nextEvent);
                
                // Add delay between tracks
                if (delayBetweenTracks > 0 && isPlaying)
                    yield return new WaitForSeconds(delayBetweenTracks);
            }
            else
            {
                // No valid track found
                yield return null;
            }
            
            // If we're not looping and have played all tracks, stop
            if (!loopMusic && playedTrackIndices.Count >= musicEvents.Count)
            {
                isPlaying = false;
            }
        }
    }
    
    /// <summary>
    /// Plays a single FMOD event with fade in/out.
    /// </summary>
    private IEnumerator PlayFMODEvent(EventReference eventRef)
    {
        // Swap current and next instances
        if (currentEventInstance.isValid())
        {
            // If we have a current instance, prepare to fade it out
            if (nextEventInstance.isValid())
            {
                nextEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                nextEventInstance.release();
            }
            
            nextEventInstance = currentEventInstance;
            StartCoroutine(FadeOutFMOD(nextEventInstance));
        }
        
        // Create and start the new event instance
        currentEventInstance = RuntimeManager.CreateInstance(eventRef);
        
        // Set initial volume to silent
        if (!string.IsNullOrEmpty(volumeParameterName))
        {
            float value;
            FMOD.RESULT result = currentEventInstance.getParameterByName(volumeParameterName, out value);
            if (result == FMOD.RESULT.OK)
            {
                currentEventInstance.setParameterByName(volumeParameterName, DbToNormalized(-80f)); // Silent
            }
            else
            {
                currentEventInstance.setVolume(0); // Silent in linear scale
            }
        }
        else
        {
            currentEventInstance.setVolume(0); // Silent in linear scale
        }
        
        currentEventInstance.start();
        
        // Fade in the new track
        StartCoroutine(FadeInFMOD(currentEventInstance));
        
        // Get the track length
        currentEventInstance.getDescription(out EventDescription description);
        description.getLength(out int length);
        float trackDuration = length / 1000f; // Convert from ms to seconds
        
        // Wait for the track to finish
        yield return new WaitForSeconds(trackDuration);
    }
    
    /// <summary>
    /// Fades in an FMOD event instance to the target volume.
    /// </summary>
    private IEnumerator FadeInFMOD(EventInstance instance)
    {
        float startDb = -80f; // Silent
        float targetDb = volumeDecibels;
        float currentTime = 0;
        
        // First check if the event has a volume parameter
        bool hasVolumeParameter = false;
        if (!string.IsNullOrEmpty(volumeParameterName))
        {
            float value;
            FMOD.RESULT result = instance.getParameterByName(volumeParameterName, out value);
            hasVolumeParameter = result == FMOD.RESULT.OK;
        }
        
        while (currentTime < fadeTime && instance.isValid())
        {
            currentTime += Time.deltaTime;
            float t = currentTime / fadeTime;
            
            // Use exponential fade for more natural volume change
            float currentDb = Mathf.Lerp(startDb, targetDb, t);
            
            // Apply volume either through parameter or direct volume
            if (hasVolumeParameter)
            {
                instance.setParameterByName(volumeParameterName, DbToNormalized(currentDb));
            }
            else
            {
                // Convert dB to linear scale for FMOD volume
                float volume = Mathf.Pow(10f, currentDb / 20f);
                instance.setVolume(volume);
            }
            
            yield return null;
        }
        
        if (instance.isValid())
        {
            // Set final volume
            if (hasVolumeParameter)
            {
                instance.setParameterByName(volumeParameterName, DbToNormalized(targetDb));
            }
            else
            {
                float volume = Mathf.Pow(10f, targetDb / 20f);
                instance.setVolume(volume);
            }
        }
    }
    
    // Helper function to convert decibels to normalized 0-1 range for FMOD parameters
    private float DbToNormalized(float db)
    {
        // Assuming parameter range is from -80dB to 0dB
        return Mathf.InverseLerp(-80f, 0f, db);
    }
    
    /// <summary>
    /// Fades out an FMOD event instance to zero volume.
    /// </summary>
    private IEnumerator FadeOutFMOD(EventInstance instance)
    {
        // Get the current volume parameter or volume level
        float startDb = volumeDecibels;
        float targetDb = -80f; // Silent
        float currentTime = 0;
        
        // Check if the event has a volume parameter
        bool hasVolumeParameter = false;
        if (!string.IsNullOrEmpty(volumeParameterName))
        {
            float paramValue;
            FMOD.RESULT result = instance.getParameterByName(volumeParameterName, out paramValue);
            hasVolumeParameter = result == FMOD.RESULT.OK;
            
            if (hasVolumeParameter)
            {
                // Convert from normalized to dB
                startDb = Mathf.Lerp(-80f, 0f, paramValue);
            }
        }
        
        if (!hasVolumeParameter)
        {
            // Get current volume if no parameter
            instance.getVolume(out float volume);
            if (volume > 0)
            {
                // Convert from linear to dB
                startDb = 20f * Mathf.Log10(volume);
            }
        }
        
        while (currentTime < fadeTime && instance.isValid())
        {
            currentTime += Time.deltaTime;
            float t = currentTime / fadeTime;
            float currentDb = Mathf.Lerp(startDb, targetDb, t);
            
            // Apply volume either through parameter or direct volume
            if (hasVolumeParameter)
            {
                instance.setParameterByName(volumeParameterName, DbToNormalized(currentDb));
            }
            else
            {
                // Convert dB to linear scale for FMOD volume
                float volume = Mathf.Pow(10f, currentDb / 20f);
                instance.setVolume(volume);
            }
            
            yield return null;
        }
        
        if (instance.isValid())
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            instance.release();
        }
    }
    
    /// <summary>
    /// Gets the index of the next track to play, ensuring we don't repeat tracks until all have been played.
    /// </summary>
    private int GetNextTrackIndex()
    {
        // If we've played all tracks, reset the history
        if (playedTrackIndices.Count >= musicEvents.Count)
        {
            playedTrackIndices.Clear();
        }
        
        // Get available tracks (those not in the played history)
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < musicEvents.Count; i++)
        {
            if (!playedTrackIndices.Contains(i))
            {
                availableIndices.Add(i);
            }
        }
        
        // Select a random track from available ones
        if (availableIndices.Count > 0)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            int selectedTrackIndex = availableIndices[randomIndex];
            
            // Add to played history
            playedTrackIndices.Add(selectedTrackIndex);
            
            return selectedTrackIndex;
        }
        
        return -1;
    }
    
    /// <summary>
    /// Sets the volume in decibels for background music.
    /// </summary>
    public void SetVolumeDecibels(float decibels)
    {
        // Clamp to reasonable range
        volumeDecibels = Mathf.Clamp(decibels, -80f, 0f);
        
        // Update currently playing FMOD events if any
        if (currentEventInstance.isValid())
        {
            // Check if the event has a volume parameter
            bool hasVolumeParameter = false;
            if (!string.IsNullOrEmpty(volumeParameterName))
            {
                float value;
                FMOD.RESULT result = currentEventInstance.getParameterByName(volumeParameterName, out value);
                hasVolumeParameter = result == FMOD.RESULT.OK;
                
                if (hasVolumeParameter)
                {
                    currentEventInstance.setParameterByName(volumeParameterName, DbToNormalized(volumeDecibels));
                }
            }
            
            if (!hasVolumeParameter)
            {
                // Convert dB to linear scale for FMOD volume
                float volume = Mathf.Pow(10f, volumeDecibels / 20f);
                currentEventInstance.setVolume(volume);
            }
        }
    }
}
