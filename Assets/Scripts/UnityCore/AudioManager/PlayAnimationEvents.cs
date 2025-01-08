using UnityEngine;
using System;
using FMODUnity;

/// <summary>
/// Handles sound playback through Unity animation events.
/// This component allows playing FMOD sounds through animation events,
/// with support for position offsets and world/local space positioning.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayAnimationEvents : MonoBehaviour
{
    #region Serialized Fields
    [Header("Settings")]
    [Tooltip("If true, all sounds from this component will be muted")]
    [SerializeField] public bool muteSounds = false;

    [Tooltip("If true, sounds will play at world position. If false, they'll play at local position")]
    [SerializeField] private bool useWorldPosition = true;

    [Tooltip("Offset from the object's position where sounds will play")]
    [SerializeField] private Vector3 soundOffset = Vector3.zero;
    
    [Header("Debug")]
    [Tooltip("If true, will log detailed debug information about sound playback")]
    [SerializeField] private bool logDebugInfo = false;
    #endregion

    #region Private Fields
    /// <summary>
    /// Cached transform component for performance optimization.
    /// </summary>
    private Transform cachedTransform;

    /// <summary>
    /// Flag indicating whether the component has been properly initialized.
    /// </summary>
    private bool isInitialized;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// Initializes the component on Awake.
    /// </summary>
    private void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Initializes the component if not already initialized.
    /// Caches required components and sets up initial state.
    /// </summary>
    private void Initialize()
    {
        if (isInitialized) return;

        try
        {
            cachedTransform = transform;
            isInitialized = true;

            if (logDebugInfo)
            {
                Debug.Log($"PlayAnimationEvents initialized on {gameObject.name}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize PlayAnimationEvents on {gameObject.name}: {e.Message}");
            enabled = false;
        }
    }
    #endregion

    #region Sound Playback Methods
    /// <summary>
    /// Plays a sound at the object's position with optional offset.
    /// This method is called through animation events.
    /// </summary>
    /// <param name="path">The FMOD event path to play</param>
    /// <remarks>
    /// The sound position is determined by:
    /// - useWorldPosition: Whether to use world or local space
    /// - soundOffset: Additional offset from the object's position
    /// The sound will not play if muteSounds is true or if the path is invalid.
    /// </remarks>
    private void PlaySound(string path)
    {
        if (!isInitialized)
        {
            Initialize();
        }

        if (muteSounds || string.IsNullOrEmpty(path)) return;

        try
        {
            Vector3 soundPosition = useWorldPosition 
                ? cachedTransform.position + soundOffset 
                : soundOffset;

            RuntimeManager.PlayOneShot(path, soundPosition);

            if (logDebugInfo)
            {
                Debug.Log($"Playing sound {path} at position {soundPosition}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to play sound {path}: {e.Message}");
        }
    }
    #endregion

    #region Public Configuration Methods
    /// <summary>
    /// Enables or disables sound playback for this component.
    /// </summary>
    /// <param name="mute">True to mute sounds, false to enable them</param>
    public void SetMute(bool mute)
    {
        muteSounds = mute;
        
        if (logDebugInfo)
        {
            Debug.Log($"Sound playback {(mute ? "muted" : "unmuted")} on {gameObject.name}");
        }
    }

    /// <summary>
    /// Sets whether sounds should use world or local position for playback.
    /// </summary>
    /// <param name="useWorld">True to use world position, false for local position</param>
    public void SetUseWorldPosition(bool useWorld)
    {
        useWorldPosition = useWorld;
        
        if (logDebugInfo)
        {
            Debug.Log($"Using {(useWorld ? "world" : "local")} position for sound playback on {gameObject.name}");
        }
    }

    /// <summary>
    /// Sets the offset from the object's position where sounds will play.
    /// </summary>
    /// <param name="offset">The position offset in world or local space (based on useWorldPosition)</param>
    public void SetSoundOffset(Vector3 offset)
    {
        soundOffset = offset;
        
        if (logDebugInfo)
        {
            Debug.Log($"Sound offset set to {offset} on {gameObject.name}");
        }
    }
    #endregion
}