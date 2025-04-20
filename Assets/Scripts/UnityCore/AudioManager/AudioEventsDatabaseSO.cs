using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityCore.AudioManager
{
    /// <summary>
    /// ScriptableObject database containing all audio event mappings organized in groups.
    /// This provides a centralized way to manage FMOD event paths without hardcoding them in scripts.
    /// </summary>
    [CreateAssetMenu(fileName = "AudioEventsDatabase", menuName = "Audio/Audio Events Database")]
    public class AudioEventsDatabaseSO : ScriptableObject
    {
        [Tooltip("List of audio event groups")]
        [SerializeField] private List<AudioEventGroup> audioGroups = new List<AudioEventGroup>();

        // Runtime lookup dictionary for quick access to event paths
        private Dictionary<string, string> eventPathsById;

        /// <summary>
        /// Initializes the lookup dictionary when the ScriptableObject becomes enabled
        /// </summary>
        private void OnEnable()
        {
            InitializeDictionary();
        }

        /// <summary>
        /// Creates a lookup dictionary for quick access to event paths by their IDs
        /// </summary>
        private void InitializeDictionary()
        {
            eventPathsById = new Dictionary<string, string>(System.StringComparer.Ordinal);
            foreach (var group in audioGroups)
            {
                foreach (var evt in group.events)
                {
                    if (string.IsNullOrEmpty(evt.id) || string.IsNullOrEmpty(evt.eventPath))
                    {
                        Debug.LogWarning($"Invalid audio event in group {group.groupName}: ID or path is empty");
                        continue;
                    }

                    string id = evt.id;
                    if (eventPathsById.ContainsKey(id))
                    {
                        Debug.LogError($"Duplicate audio event ID '{id}' in group {group.groupName}");
                        continue;
                    }

                    eventPathsById[id] = evt.eventPath;
                }
            }
        }

        /// <summary>
        /// Gets the FMOD event path for the specified audio event ID
        /// </summary>
        /// <param name="id">The ID of the audio event</param>
        /// <returns>The FMOD event path if found, null otherwise</returns>
        public string GetEventPath(string id)
        {
            if (eventPathsById == null)
            {
                InitializeDictionary();
            }

            return eventPathsById.TryGetValue(id, out string path) ? path : null;
        }

        public string GetEventPath(AudioEventId eventId)
        {
            return GetEventPath(eventId.ToString());
        }

        /// <summary>
        /// Gets all event IDs from all groups. Used for enum generation.
        /// </summary>
        public IEnumerable<string> GetAllEventIds()
        {
            return audioGroups
                .SelectMany(group => group.events)
                .Select(evt => evt.id);
        }

        /// <summary>
        /// Validates that all event IDs and paths are properly set
        /// </summary>
        private void OnValidate()
        {
            foreach (var group in audioGroups)
            {
                foreach (var evt in group.events)
                {
                    if (string.IsNullOrEmpty(evt.id))
                    {
                        Debug.LogWarning($"Empty event ID found in group {group.groupName}");
                    }
                    if (string.IsNullOrEmpty(evt.eventPath))
                    {
                        Debug.LogWarning($"Empty event path found for ID {evt.id} in group {group.groupName}");
                    }
                }
            }
        }
    }
}
