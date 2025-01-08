using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ensures that the attached GameObject is not destroyed when loading a new scene.
/// Detaches the GameObject from its parent to avoid unexpected behavior.
/// </summary>
public class DontDestroyOnLoad : MonoBehaviour
{
    /// <summary>
    /// Called when the script instance is being loaded.
    /// Detaches the GameObject from its parent and marks it to not be destroyed on scene load.
    /// </summary>
    private void Awake()
    {
        // Detach the GameObject from its parent to ensure it remains in the new scene.
        transform.SetParent(null);

        // Mark the GameObject to not be destroyed on scene load.
        Object.DontDestroyOnLoad(gameObject);
    }
}
