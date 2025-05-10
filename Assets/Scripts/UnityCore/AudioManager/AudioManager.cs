using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityCore.AudioManager;

/// <summary>
/// Core audio management system that handles FMOD audio integration.
/// Provides centralized control over audio playback, volume settings, and bus management.
/// Implements the singleton pattern to ensure only one instance exists.
/// </summary>
[DefaultExecutionOrder(-80)]
public class AudioManager : MonoBehaviour
{
    #region Volume Settings
    /// <summary>
    /// Contains volume settings for different audio categories.
    /// Values are normalized between 0 (muted) and 1 (full volume).
    /// </summary>
    [Serializable]
    public class VolumeSettings
    {
        [Range(0, 1)] public float master = 1f;
        [Range(0, 1)] public float music = 1f;
        [Range(0, 1)] public float sfx = 1f;
        [Range(0, 1)] public float sfx2 = 1f;
    }

    [Header("Volume Settings")]
    [Tooltip("Volume levels for different audio categories")]
    [SerializeField] private VolumeSettings volumeSettings = new VolumeSettings();

    [Header("Debug Settings")]
    [Tooltip("If true, enables detailed logging of audio operations")]
    [SerializeField] protected bool logDebugInfo = false;

    [Header("Audio Events")]
    [Tooltip("Database containing all audio event mappings")]
    [SerializeField] private AudioEventsDatabaseSO audioEventsDatabase;
    #endregion

    #region Private Fields
    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;
    private Bus sfx2Bus;

    protected readonly List<EventInstance> eventInstances = new List<EventInstance>();
    private readonly List<StudioEventEmitter> eventEmitters = new List<StudioEventEmitter>();

    private EventInstance musicEventInstance;
    #endregion

    #region Public Properties
    /// <summary>
    /// Singleton instance of the AudioManager.
    /// </summary>
    public static AudioManager instance { get; private set; }

    // Volume Properties (Backward Compatibility)
    /// <summary>
    /// Master volume level (0-1). Affects all audio output.
    /// </summary>
    public float masterVolume
    {
        get => volumeSettings.master;
        set => volumeSettings.master = value;
    }

    /// <summary>
    /// Music volume level (0-1). Affects background music.
    /// </summary>
    public float musicVolume
    {
        get => volumeSettings.music;
        set => volumeSettings.music = value;
    }

    /// <summary>
    /// Sound effects volume level (0-1). Affects primary SFX channel.
    /// </summary>
    public float SFXVolume
    {
        get => volumeSettings.sfx;
        set => volumeSettings.sfx = value;
    }

    /// <summary>
    /// Secondary sound effects volume level (0-1). Affects secondary SFX channel.
    /// </summary>
    public float SFX2Volume
    {
        get => volumeSettings.sfx2;
        set => volumeSettings.sfx2 = value;
    }
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the singleton instance and sets up FMOD buses.
    /// </summary>
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError($"Found more than one Audio Manager in the scene. Destroying duplicate at {gameObject.name}");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeBuses();
    }

    /// <summary>
    /// Initializes FMOD audio buses and sets up volume controls.
    /// Master bus is required, while other buses are optional.
    /// </summary>
    private void InitializeBuses()
    {
        try
        {
            // Master bus is required
            masterBus = RuntimeManager.GetBus("bus:/");
            if (!IsBusValid(masterBus))
            {
                throw new Exception("Master bus not found or invalid");
            }

            // Try to get other buses, but don't fail if they don't exist
            try { musicBus = RuntimeManager.GetBus("bus:/Music"); } 
            catch { Debug.LogWarning("Music bus not found at 'bus:/Music'. Music volume control will be disabled."); }

            try { sfxBus = RuntimeManager.GetBus("bus:/SFX"); }
            catch { Debug.LogWarning("SFX bus not found at 'bus:/SFX'. SFX volume control will be disabled."); }

            try { sfx2Bus = RuntimeManager.GetBus("bus:/SFX2"); }
            catch { Debug.LogWarning("SFX2 bus not found at 'bus:/SFX2'. SFX2 volume control will be disabled."); }

            if (logDebugInfo)
            {
                LogActiveBuses();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize audio buses: {e.Message}");
            enabled = false; // Disable the component if master bus initialization fails
        }
    }

    /// <summary>
    /// Logs the status of all audio buses for debugging purposes.
    /// </summary>
    private void LogActiveBuses()
    {
        Debug.Log("Active Audio Buses:");
        if (IsBusValid(masterBus)) Debug.Log(" - Master Bus: Active");
        if (IsBusValid(musicBus)) Debug.Log(" - Music Bus: Active");
        if (IsBusValid(sfxBus)) Debug.Log(" - SFX Bus: Active");
        if (IsBusValid(sfx2Bus)) Debug.Log(" - SFX2 Bus: Active");
    }

    /// <summary>
    /// Waits for GameEvents to be available and subscribes to relevant events.
    /// </summary>
    private IEnumerator WaitToSubscribe()
    {
        while (GameEvents.Instance == null)
        {
            yield return null;
        }

        GameEvents.Instance.onPauseTrigger += PauseSFX;
        
        if (logDebugInfo)
        {
            Debug.Log("Audio Manager subscribed to game events");
        }
    }

    private void OnEnable()
    {
        StartCoroutine(WaitToSubscribe());
    }

    private void OnDisable()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onPauseTrigger -= PauseSFX;
        }
    }
    #endregion

    #region Audio Control Methods
    /// <summary>
    /// Pauses or unpauses all sound effects.
    /// </summary>
    /// <param name="pause">True to pause, false to unpause</param>
    private void PauseSFX(bool pause)
    {
        try
        {
            if (IsBusValid(sfxBus))
            {
                sfxBus.setPaused(pause);
                if (logDebugInfo)
                {
                    Debug.Log($"SFX bus {(pause ? "paused" : "resumed")}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to {(pause ? "pause" : "resume")} SFX bus: {e.Message}");
        }
    }

    /// <summary>
    /// Stops all currently playing sound effects.
    /// </summary>
    public void ClearSFXBus()
    {
        try
        {
            if (IsBusValid(sfxBus))
            {
                sfxBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
                if (logDebugInfo)
                {
                    Debug.Log("SFX bus cleared");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to clear SFX bus: {e.Message}");
        }
    }

    private void Update()
    {
        UpdateBusVolumes();
    }

    /// <summary>
    /// Checks if an FMOD bus is valid and can be used.
    /// </summary>
    /// <param name="bus">The bus to check</param>
    /// <returns>True if the bus is valid and has a handle</returns>
    private bool IsBusValid(Bus bus)
    {
        return bus.hasHandle();
    }

    /// <summary>
    /// Updates the volume levels of all active audio buses.
    /// </summary>
    private void UpdateBusVolumes()
    {
        try
        {
            if (IsBusValid(masterBus)) masterBus.setVolume(volumeSettings.master);
            if (IsBusValid(musicBus)) musicBus.setVolume(volumeSettings.music);
            if (IsBusValid(sfxBus)) sfxBus.setVolume(volumeSettings.sfx);
            if (IsBusValid(sfx2Bus)) sfx2Bus.setVolume(volumeSettings.sfx2);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to update bus volumes: {e.Message}");
        }
    }

    /// <summary>
    /// Plays background music from an audio event ID.
    /// </summary>
    /// <param name="musicEventId">The ID of the music event to play</param>
    /// <param name="stopCurrent">If true, stops currently playing music first</param>
    public void PlayMusic(AudioEventId musicEventId, bool stopCurrent = true)
    {
        try
        {
            if (stopCurrent)
            {
                StopMusic();
            }

            string eventPath = audioEventsDatabase.GetEventPath(musicEventId);
            if (string.IsNullOrEmpty(eventPath))
            {
                Debug.LogWarning($"Music event ID not found: {musicEventId}");
                return;
            }

            musicEventInstance = RuntimeManager.CreateInstance(eventPath);
            eventInstances.Add(musicEventInstance);
            musicEventInstance.start();

            if (logDebugInfo)
            {
                Debug.Log($"Playing music: {musicEventId} at path: {eventPath}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to play music: {e.Message}");
        }
    }

    /// <summary>
    /// Legacy method for playing music using FMOD EventReference.
    /// Consider using PlayMusic(AudioEventId) instead.
    /// </summary>
    public void PlayMusic(EventReference musicEventReference, bool stopCurrent = true)
    {
        try
        {
            if (stopCurrent)
            {
                StopMusic();
            }

            musicEventInstance = RuntimeManager.CreateInstance(musicEventReference);
            eventInstances.Add(musicEventInstance);
            musicEventInstance.start();

            if (logDebugInfo)
            {
                Debug.Log($"Playing music: {musicEventReference}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to play music: {e.Message}");
        }
    }

    /// <summary>
    /// Stops currently playing background music.
    /// </summary>
    public void StopMusic()
    {
        try
        {
            if (musicEventInstance.isValid())
            {
                musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                musicEventInstance.release();

                if (logDebugInfo)
                {
                    Debug.Log("Music stopped");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to stop music: {e.Message}");
        }
    }



    /// <summary>
    /// Initializes an FMOD event emitter on a GameObject.
    /// </summary>
    /// <param name="eventReference">The FMOD event for the emitter</param>
    /// <param name="emitterGameObject">The GameObject to attach the emitter to</param>
    /// <returns>The initialized StudioEventEmitter component</returns>
    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        try
        {
            StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
            if (emitter == null)
            {
                emitter = emitterGameObject.AddComponent<StudioEventEmitter>();
            }

            emitter.EventReference = eventReference;
            eventEmitters.Add(emitter);

            if (logDebugInfo)
            {
                Debug.Log($"Initialized event emitter on {emitterGameObject.name}: {eventReference.ToString()}");
            }

            return emitter;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize event emitter: {e.Message}");
            throw;
        }
    }
    #endregion

    #region Cleanup Methods
    /// <summary>
    /// Cleans up all audio resources, stopping and releasing event instances.
    /// </summary>
    private void CleanUp()
    {
        try
        {
            foreach (EventInstance eventInstance in eventInstances)
            {
                if (eventInstance.isValid())
                {
                    eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    eventInstance.release();
                }
            }
            eventInstances.Clear();

            foreach (StudioEventEmitter emitter in eventEmitters)
            {
                if (emitter != null)
                {
                    emitter.Stop();
                }
            }
            eventEmitters.Clear();

            if (logDebugInfo)
            {
                Debug.Log("Audio Manager cleaned up successfully");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to clean up Audio Manager: {e.Message}");
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
    #endregion

    #region Volume Persistence
    /// <summary>
    /// Saves current volume settings to PlayerPrefs.
    /// </summary>
    public void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", volumeSettings.master);
        PlayerPrefs.SetFloat("MusicVolume", volumeSettings.music);
        PlayerPrefs.SetFloat("SFXVolume", volumeSettings.sfx);
        PlayerPrefs.SetFloat("SFX2Volume", volumeSettings.sfx2);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads volume settings from PlayerPrefs.
    /// </summary>
    public void LoadVolumeSettings()
    {
        volumeSettings.master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSettings.music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        volumeSettings.sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);
        volumeSettings.sfx2 = PlayerPrefs.GetFloat("SFX2Volume", 1f);
    }
    #endregion

    #region Audio Events Methods
    /// <summary>
    /// Plays a one-shot sound using its event ID from the database
    /// </summary>
    /// <param name="eventId">The ID of the audio event to play</param>
    /// <param name="position">World position to play the sound at</param>
    public void PlaySound(AudioEventId eventId, Vector3 position)
    {
        string eventPath = audioEventsDatabase.GetEventPath(eventId);
        if (string.IsNullOrEmpty(eventPath))
        {
            Debug.LogWarning($"Audio event ID not found: {eventId}");
            return;
        }

        RuntimeManager.PlayOneShot(eventPath, position);
        if (logDebugInfo)
        {
            Debug.Log($"Playing sound: {eventId} at path: {eventPath}");
        }
    }

    /// <summary>
    /// Creates an FMOD EventInstance for the specified event ID
    /// </summary>
    /// <param name="eventId">The ID of the audio event</param>
    /// <returns>An FMOD EventInstance if found, default otherwise</returns>
    public EventInstance CreateEventInstance(AudioEventId eventId)
    {
        string eventPath = audioEventsDatabase.GetEventPath(eventId);
        if (string.IsNullOrEmpty(eventPath))
        {
            Debug.LogWarning($"Audio event ID not found: {eventId}");
            return default;
        }

        var instance = RuntimeManager.CreateInstance(eventPath);
        eventInstances.Add(instance);
        
        if (logDebugInfo)
        {
            Debug.Log($"Created event instance: {eventId} at path: {eventPath}");
        }

        return instance;
    }

    /// <summary>
    /// Plays a one-shot sound at the camera's position (useful for UI sounds)
    /// </summary>
    /// <param name="eventId">The ID of the audio event to play</param>
    public void PlaySound(AudioEventId eventId)
    {
        if (Camera.main != null)
        {
            PlaySound(eventId, Camera.main.transform.position);
        }
        else
        {
            PlaySound(eventId, Vector3.zero);
        }
    }


    #endregion
}