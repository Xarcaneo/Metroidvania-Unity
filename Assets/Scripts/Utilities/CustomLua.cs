using PixelCrushers.DialogueSystem;
using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Provides custom Lua functions that can be called from the Dialogue System.
/// Registers and unregisters functions with the Lua environment on enable/disable.
/// </summary>
public class CustomLua : MonoBehaviour
{
    private MethodInfo cameraNewTargetMethod;
    private MethodInfo triggerStateChangedMethod;
    private MethodInfo playAnimationMethod;
    private MethodInfo triggerSceneChangeMethod;

    /// <summary>
    /// Registers custom Lua functions when the component is enabled.
    /// </summary>
    void Awake()
    {
        // Cache MethodInfo objects to reduce runtime overhead.
        cameraNewTargetMethod = SymbolExtensions.GetMethodInfo(() => CameraNewTarget(string.Empty));
        triggerStateChangedMethod = SymbolExtensions.GetMethodInfo(() => TriggerStateChanged(string.Empty));
        playAnimationMethod = SymbolExtensions.GetMethodInfo(() => PlayAnimation(string.Empty, string.Empty));
        triggerSceneChangeMethod = SymbolExtensions.GetMethodInfo(() => TriggerSceneChange(string.Empty, string.Empty));

        Lua.RegisterFunction(nameof(CameraNewTarget), this, cameraNewTargetMethod);
        Lua.RegisterFunction(nameof(TriggerStateChanged), this, triggerStateChangedMethod);
        Lua.RegisterFunction(nameof(PlayAnimation), this, playAnimationMethod);
        Lua.RegisterFunction(nameof(TriggerSceneChange), this, triggerSceneChangeMethod);
    }

    /// <summary>
    /// Unregisters custom Lua functions when the component is disabled.
    /// </summary>
    void OnDisable()
    {
        Lua.UnregisterFunction(nameof(CameraNewTarget));
        Lua.UnregisterFunction(nameof(TriggerStateChanged));
        Lua.UnregisterFunction(nameof(PlayAnimation));
        Lua.UnregisterFunction(nameof(TriggerSceneChange));
    }

    /// <summary>
    /// Triggers a new camera target event.
    /// </summary>
    /// <param name="cameraHookID">The ID of the camera target to switch to.</param>
    private void CameraNewTarget(string cameraHookID)
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.CameraNewTarget(cameraHookID);
        }
        else
        {
            Debug.LogError("GameEvents.Instance is null. Cannot trigger CameraNewTarget.");
        }
    }

    /// <summary>
    /// Updates the trigger state in Lua and raises a corresponding game event.
    /// </summary>
    /// <param name="triggerID">The ID of the trigger to update.</param>
    private void TriggerStateChanged(string triggerID)
    {
        DialogueLua.SetVariable($"State.{triggerID}", true);
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.TriggerStateChanged(triggerID);
        }
        else
        {
            Debug.LogError("GameEvents.Instance is null. Cannot trigger TriggerStateChanged.");
        }
    }

    /// <summary>
    /// Plays an animation on a specified GameObject.
    /// </summary>
    /// <param name="objectName">The name of the GameObject to animate.</param>
    /// <param name="animationName">The name of the animation to play.</param>
    public void PlayAnimation(string objectName, string animationName)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj != null)
        {
            if (obj.TryGetComponent(out Animator animator))
            {
                animator.Play(animationName);
            }
            else
            {
                Debug.LogError($"Animator component not found on the object: {objectName}");
            }
        }
        else
        {
            Debug.LogError($"Object not found: {objectName}");
        }
    }

    #region Scene Change Event

    /// <summary>
    /// Triggers a scene change event with the destination scene and spawn point.
    /// </summary>
    /// <param name="destinationSceneName">The name of the destination scene.</param>
    /// <param name="spawnpointName">The spawn point in the destination scene.</param>
    public void TriggerSceneChange(string destinationSceneName, string spawnpointName)
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.TriggerSceneChange(destinationSceneName, spawnpointName);
        }
        else
        {
            Debug.LogError("GameEvents.Instance or onSceneChangeTriggered is null. Cannot trigger scene change.");
        }
    }

    #endregion
}
