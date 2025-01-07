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

    /// <summary>
    /// Registers custom Lua functions when the component is enabled.
    /// </summary>
    void Awake()
    {
        // Cache MethodInfo objects to reduce runtime overhead.
        cameraNewTargetMethod = SymbolExtensions.GetMethodInfo(() => CameraNewTarget(string.Empty));
        triggerStateChangedMethod = SymbolExtensions.GetMethodInfo(() => TriggerStateChanged(0));
        playAnimationMethod = SymbolExtensions.GetMethodInfo(() => PlayAnimation(string.Empty, string.Empty));

        Lua.RegisterFunction(nameof(CameraNewTarget), this, cameraNewTargetMethod);
        Lua.RegisterFunction(nameof(TriggerStateChanged), this, triggerStateChangedMethod);
        Lua.RegisterFunction(nameof(PlayAnimation), this, playAnimationMethod);
    }

    /// <summary>
    /// Unregisters custom Lua functions when the component is disabled.
    /// </summary>
    void OnDisable()
    {
        Lua.UnregisterFunction(nameof(CameraNewTarget));
        Lua.UnregisterFunction(nameof(TriggerStateChanged));
        Lua.UnregisterFunction(nameof(PlayAnimation));
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
    private void TriggerStateChanged(double triggerID)
    {
        DialogueLua.SetVariable($"Trigger.{triggerID}", true);
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.TriggerStateChanged((int)triggerID);
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
}
