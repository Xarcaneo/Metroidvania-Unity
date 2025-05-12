using UnityEngine;
using PixelCrushers.DialogueSystem;
using Opsive.Shared.Audio;
using FMODUnity;
using System.Collections;

/// <summary>
/// Bridge component that connects Pixel Crushers' TextMeshProTypewriterEffect with FMOD audio system.
/// This allows typewriter text effects to play sounds through FMOD with improved timing control.
/// </summary>
public class TypewriterSoundBridge : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The FMOD event to play for each character typed")]
    private EventReference typewriterSoundEvent;
    
    [SerializeField]
    [Tooltip("Alternative FMOD events to randomly select from (optional)")]
    private EventReference[] alternativeSoundEvents;
    
    [SerializeField]
    [Tooltip("Characters that should not play a sound (spaces, punctuation, etc.)")]
    private string silentCharacters = " ,.;:!?\"'()[]{}<>";
    
    [SerializeField]
    [Tooltip("Reference to the TextMeshProTypewriterEffect component")]
    private TextMeshProTypewriterEffect typewriterEffect;
    
    [Header("Sound Timing")]
    
    [SerializeField]
    [Tooltip("Play sound only every X characters (1 = every character, 2 = every other character, etc.)")]
    private int playFrequency = 1;
    
    [SerializeField]
    [Tooltip("Minimum time between sounds in seconds (prevents sound overlap)")]
    private float minTimeBetweenSounds = 0.05f;
    
    [SerializeField]
    [Tooltip("Maximum number of simultaneous sounds (0 = unlimited)")]
    private int maxSimultaneousSounds = 3;
    
    // Private variables for sound management
    private FMOD.Studio.EventInstance currentEventInstance;
    private float lastSoundTime;
    private int characterCounter;
    private int activeSoundCount;
    private bool isInitialized;

    private void Awake()
    {
        // If typewriter effect wasn't assigned in the inspector, try to get it
        if (typewriterEffect == null)
        {
            typewriterEffect = GetComponent<TextMeshProTypewriterEffect>();
            
            if (typewriterEffect == null)
            {
                Debug.LogError("TypewriterSoundBridge requires a TextMeshProTypewriterEffect component on the same GameObject.");
                return;
            }
        }
        
        // Initialize variables
        lastSoundTime = 0f;
        characterCounter = 0;
        activeSoundCount = 0;
        
        // Disable the built-in audio in the typewriter effect
        typewriterEffect.audioClip = null;
        typewriterEffect.audioSource = null;
        
        // Set the silent characters to match our settings
        typewriterEffect.silentCharacters = silentCharacters;
        
        // Subscribe to events
        typewriterEffect.onCharacter.AddListener(OnCharacterTyped);
        typewriterEffect.onBegin.AddListener(OnTypewriterBegin);
        typewriterEffect.onEnd.AddListener(OnTypewriterEnd);
        
        isInitialized = true;
    }
    
    private void OnTypewriterBegin()
    {
        // Reset counters when typewriter starts
        characterCounter = 0;
        activeSoundCount = 0;
        lastSoundTime = Time.time;
    }
    
    private void OnTypewriterEnd()
    {
        // Clean up any remaining sounds
        StopAllSounds();
    }

    /// <summary>
    /// Called when a character is typed by the typewriter effect
    /// </summary>
    private void OnCharacterTyped()
    {
        characterCounter++;
        
        // Check if we should play a sound for this character
        if (ShouldPlaySound())
        {
            PlayTypewriterSound();
        }
    }
    
    /// <summary>
    /// Determines if a sound should be played for the current character
    /// </summary>
    private bool ShouldPlaySound()
    {
        // Skip if no sound event is assigned
        if (typewriterSoundEvent.IsNull)
            return false;
            
        // Check if character is within the play frequency
        if (characterCounter % playFrequency != 0)
            return false;
            
        // Check if enough time has passed since the last sound
        if (Time.time - lastSoundTime < minTimeBetweenSounds)
            return false;
            
        // Check if we've reached the maximum number of simultaneous sounds
        if (maxSimultaneousSounds > 0 && activeSoundCount >= maxSimultaneousSounds)
            return false;
            
        return true;
    }

    /// <summary>
    /// Plays the typewriter sound using FMOD
    /// </summary>
    private void PlayTypewriterSound()
    {
        // Select a random sound event if alternatives are available
        EventReference soundToPlay = typewriterSoundEvent;
        if (alternativeSoundEvents != null && alternativeSoundEvents.Length > 0)
        {
            int randomIndex = Random.Range(0, alternativeSoundEvents.Length + 1);
            if (randomIndex < alternativeSoundEvents.Length)
            {
                soundToPlay = alternativeSoundEvents[randomIndex];
            }
        }
        
        // Create and play a new FMOD event instance
        StartCoroutine(PlaySoundWithCleanup(soundToPlay));
        
        // Update timing variables
        lastSoundTime = Time.time;
    }
    
    /// <summary>
    /// Plays a sound and handles its cleanup when finished
    /// </summary>
    private IEnumerator PlaySoundWithCleanup(EventReference soundEvent)
    {
        activeSoundCount++;
        
        FMOD.Studio.EventInstance eventInstance = RuntimeManager.CreateInstance(soundEvent);
        eventInstance.start();
        
        // Get the length of the event
        eventInstance.getDescription(out FMOD.Studio.EventDescription description);
        description.getLength(out int length);
        float soundDuration = length / 1000f; // Convert from milliseconds to seconds
        
        // Wait for the sound to finish
        yield return new WaitForSeconds(soundDuration);
        
        // Clean up the event instance
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
        activeSoundCount--;
    }
    
    /// <summary>
    /// Stops all currently playing sounds
    /// </summary>
    private void StopAllSounds()
    {
        // Stop all coroutines to prevent further sound playback
        StopAllCoroutines();
        
        // Reset counters
        activeSoundCount = 0;
    }

    private void OnDestroy()
    {
        if (!isInitialized) return;
        
        // Unsubscribe from events
        if (typewriterEffect != null)
        {
            typewriterEffect.onCharacter.RemoveListener(OnCharacterTyped);
            typewriterEffect.onBegin.RemoveListener(OnTypewriterBegin);
            typewriterEffect.onEnd.RemoveListener(OnTypewriterEnd);
        }
        
        // Clean up any playing sounds
        StopAllSounds();
    }
}
