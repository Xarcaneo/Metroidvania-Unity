using UnityEngine;
using PixelCrushers;

public class SceneChangeHandler : MonoBehaviour
{
    private void Start()
    {
        // Subscribe to the scene change event.
        GameEvents.Instance.onSceneChangeTriggered += OnSceneChangeTriggered;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks.
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.onSceneChangeTriggered -= OnSceneChangeTriggered;
        }
    }

    /// <summary>
    /// Handles the scene change event by creating and configuring a ScenePortal instance.
    /// </summary>
    /// <param name="destinationSceneName">The destination scene name.</param>
    /// <param name="spawnpointName">The spawn point name in the destination scene.</param>
    private void OnSceneChangeTriggered(string destinationSceneName, string spawnpointName)
    {
        // Dynamically create a new ScenePortal instance.
        GameObject portalObject = new GameObject("DynamicScenePortal");
        ScenePortal portal = portalObject.AddComponent<ScenePortal>();

        // Set the required destination variables.
        portal.SetDestination(destinationSceneName, spawnpointName);

        // Trigger the portal's UsePortal method.
        portal.UsePortal();
    }
}
