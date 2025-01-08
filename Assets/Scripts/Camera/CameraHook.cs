using System;
using UnityEngine;

/// <summary>
/// Identifies a point of interest for camera focus in the game world.
/// Used by the CameraController to determine camera positioning and movement targets.
/// </summary>
/// <remarks>
/// Camera hooks are used to:
/// - Mark important locations for camera focus
/// - Enable dynamic camera target switching
/// - Create cinematic sequences
/// 
/// Each hook must have a unique ID for identification during camera transitions.
/// </remarks>
public class CameraHook : MonoBehaviour
{
    /// <summary>
    /// Unique identifier for this camera hook point
    /// </summary>
    [SerializeField]
    [Tooltip("Unique ID used to identify this camera hook during target switching")]
    public String HookID;
}
