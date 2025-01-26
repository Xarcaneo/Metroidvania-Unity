using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Utility class for handling scene-related operations.
/// </summary>
public static class SceneUtils
{
    // Dictionary to cache scene names by build index
    private static Dictionary<int, string> sceneNameCache = new Dictionary<int, string>();

    /// <summary>
    /// Gets the scene name from a build index.
    /// </summary>
    /// <param name="buildIndex">The build index of the scene</param>
    /// <returns>The name of the scene, or null if not found</returns>
    public static string GetSceneNameFromBuildIndex(int buildIndex)
    {
        // Try to get from cache first
        if (sceneNameCache.TryGetValue(buildIndex, out string sceneName))
        {
            return sceneName;
        }

        // If not in cache, try to get from build settings
        try
        {
            string path = SceneUtility.GetScenePathByBuildIndex(buildIndex);
            if (!string.IsNullOrEmpty(path))
            {
                // Extract scene name from path (removes the path and extension)
                sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
                sceneNameCache[buildIndex] = sceneName;
                return sceneName;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting scene name for build index {buildIndex}: {e.Message}");
        }

        return null;
    }

    /// <summary>
    /// Gets the scene name from a build index string.
    /// </summary>
    /// <param name="buildIndexStr">The build index as a string</param>
    /// <returns>The name of the scene, or null if invalid input or scene not found</returns>
    public static string GetSceneNameFromBuildIndex(string buildIndexStr)
    {
        if (int.TryParse(buildIndexStr, out int buildIndex))
        {
            return GetSceneNameFromBuildIndex(buildIndex);
        }
        
        Debug.LogError($"Invalid scene build index: {buildIndexStr}");
        return null;
    }

    /// <summary>
    /// Gets the current scene name.
    /// </summary>
    /// <returns>The name of the active scene</returns>
    public static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
}
