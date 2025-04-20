using System;
using UnityEngine;

namespace UnityCore.AudioManager
{
    /// <summary>
    /// Represents a single FMOD audio event mapping with an identifier and event path
    /// </summary>
    [Serializable]
    public class AudioEvent
    {
        [Tooltip("Unique identifier for the audio event")]
        public string id;
        
        [Tooltip("FMOD event path (e.g., 'event:/Characters/Player/Jump')")]
        public string eventPath;
    }
}
