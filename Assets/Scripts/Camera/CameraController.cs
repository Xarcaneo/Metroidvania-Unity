using Com.LuisPedroFonseca.ProCamera2D;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Controls camera behavior and target management using ProCamera2D.
/// Handles dynamic switching between different camera targets and responds to game events.
/// </summary>
/// <remarks>
/// This controller manages:
/// - Multiple camera targets through CameraHook components
/// - Player tracking after spawn
/// - Dynamic target switching based on game events
/// 
/// Uses the ProCamera2D asset for advanced camera functionality.
/// </remarks>
public class CameraController : MonoBehaviour
{
    #region Private Fields
    /// <summary>
    /// Reference to the ProCamera2D component for camera control
    /// </summary>
    private ProCamera2D m_proCamera2D;
    #endregion

    #region Public Fields
    /// <summary>
    /// List of all current camera targets in the scene
    /// </summary>
    [HideInInspector]
    public List<GameObject> targets = new List<GameObject>();
    #endregion

    /// <summary>
    /// Initializes the camera controller and sets up event subscriptions
    /// </summary>
    void Awake()
    {
        // Find all camera hook targets in the scene
        CameraHook[] cameraTargets = FindObjectsOfType<CameraHook>();
        foreach (CameraHook target in cameraTargets)
        {
            targets.Add(target.gameObject);
        }

        // Subscribe to relevant game events
        GameEvents.Instance.onPlayerSpawned += OnPlayerSpawned;
        GameEvents.Instance.onCameraNewTarget += OnCameraNewTarget;

        // Get required components
        m_proCamera2D = GetComponent<ProCamera2D>();
    }

    /// <summary>
    /// Cleans up event subscriptions when the controller is destroyed
    /// </summary>
    void OnDestroy()
    {
        // Unsubscribe from game events
        GameEvents.Instance.onPlayerSpawned -= OnPlayerSpawned;
        GameEvents.Instance.onCameraNewTarget -= OnCameraNewTarget;
    }

    /// <summary>
    /// Handles camera target switching when a new target is requested
    /// </summary>
    /// <param name="hookID">ID of the CameraHook to focus on</param>
    /// <remarks>
    /// This method:
    /// 1. Finds the requested camera hook by ID
    /// 2. Clears all current camera targets
    /// 3. Sets the new target as the focus point
    /// </remarks>
    private void OnCameraNewTarget(string hookID)
    {
        // Find target hook by ID
        CameraHook target = FindObjectsOfType<CameraHook>()
            .FirstOrDefault(t => t.HookID == hookID);

        // Switch camera focus to new target
        m_proCamera2D.RemoveAllCameraTargets();
        m_proCamera2D.AddCameraTarget(target.transform);
    }

    /// <summary>
    /// Handles camera behavior when the player spawns
    /// </summary>
    /// <remarks>
    /// Sets the player as a camera target and centers the view on them
    /// </remarks>
    void OnPlayerSpawned()
    {
        m_proCamera2D.AddCameraTarget(Player.Instance.transform);
        m_proCamera2D.CenterOnTargets();
    }
}
