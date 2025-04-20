using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityCore.AudioManager
{
    /// <summary>
    /// Represents a group of related audio events (e.g., all player sounds, all UI sounds)
    /// </summary>
    [Serializable]
    public class AudioEventGroup
    {
        [Tooltip("Name of the audio group (e.g., 'Player', 'UI')")]
        public string groupName;
        
        [Tooltip("List of audio events in this group")]
        public List<AudioEvent> events = new List<AudioEvent>();
    }
}
