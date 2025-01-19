using UnityEngine;
using System;
using PixelCrushers;

/// <summary>
/// Extension of SpawnedObject that preserves parent-child relationships and a custom integer value
/// when objects are saved and restored. This component should be used instead of the base SpawnedObject
/// when you need to maintain parent relationships or store additional data with spawned objects.
/// </summary>
public class SpawnedObjectParentSaver : SpawnedObject
{
    /// <summary>
    /// Custom integer value that will be saved and restored with the object.
    /// This can be used to store any type of numeric data that needs to persist.
    /// </summary>
    [SerializeField]
    [Tooltip("Custom integer value that will be saved and restored with the object")]
    private int customIntValue;

    /// <summary>
    /// Data structure for serializing the spawned object's state.
    /// </summary>
    [Serializable]
    private class SaveData
    {
        /// <summary>
        /// Name of the parent object this spawned object is attached to.
        /// </summary>
        public string parentName;

        /// <summary>
        /// Custom integer value associated with this object.
        /// </summary>
        public int customValue;

        /// <summary>
        /// Serialized data from the base SpawnedObject class.
        /// </summary>
        public string baseData; // To store the base SpawnedObject data
    }

    /// <summary>
    /// Gets or sets the custom integer value associated with this spawned object.
    /// This value will be preserved when the game is saved and restored.
    /// </summary>
    public int CustomIntValue
    {
        get { return customIntValue; }
        set { customIntValue = value; }
    }

    /// <summary>
    /// Records the current state of the spawned object, including its parent relationship,
    /// custom integer value, and base SpawnedObject data.
    /// </summary>
    /// <returns>A serialized string containing the object's complete state data.</returns>
    public override string RecordData()
    {
        var data = new SaveData();
        
        // Save the parent's name if we have one
        if (transform.parent != null)
        {
            data.parentName = transform.parent.name;
        }
        
        // Save our custom int value
        data.customValue = customIntValue;

        // Save base SpawnedObject data
        data.baseData = base.RecordData();

        return SaveSystem.Serialize(data);
    }

    /// <summary>
    /// Restores the state of the spawned object from saved data, including its parent relationship,
    /// custom integer value, and base SpawnedObject data.
    /// </summary>
    /// <param name="s">The serialized string containing the object's saved state data.</param>
    public override void ApplyData(string s)
    {
        if (string.IsNullOrEmpty(s)) return;

        var data = SaveSystem.Deserialize<SaveData>(s);
        if (data == null) return;

        // Restore the custom int value
        customIntValue = data.customValue;

        // Find and set the parent if we have one
        if (!string.IsNullOrEmpty(data.parentName))
        {
            var parent = GameObject.Find(data.parentName);
            if (parent != null)
            {
                transform.SetParent(parent.transform, true);
            }
        }

        // Apply base SpawnedObject data
        if (!string.IsNullOrEmpty(data.baseData))
        {
            base.ApplyData(data.baseData);
        }
    }
}
