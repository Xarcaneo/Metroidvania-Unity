using PixelCrushers.DialogueSystem;
using System;
using UnityEngine;

public class CustomLua : MonoBehaviour
{
    void OnEnable()
    {
        Lua.RegisterFunction(nameof(CameraNewTarget), this, SymbolExtensions.GetMethodInfo(() => CameraNewTarget(string.Empty)));
        Lua.RegisterFunction(nameof(TriggerStateChanged), this, SymbolExtensions.GetMethodInfo(() => TriggerStateChanged(0)));
        Lua.RegisterFunction(nameof(PlayAnimation), this, SymbolExtensions.GetMethodInfo(() => PlayAnimation(string.Empty, string.Empty)));
    }

    void OnDisable()
    {
        Lua.UnregisterFunction(nameof(CameraNewTarget)); 
        Lua.UnregisterFunction(nameof(TriggerStateChanged));
        Lua.UnregisterFunction(nameof(PlayAnimation));
    }

    private void CameraNewTarget(String cameraHookID) => GameEvents.Instance.CameraNewTarget(cameraHookID);
    private void TriggerStateChanged(double triggerID)
    {
        DialogueLua.SetVariable("Trigger." + triggerID, true);
        GameEvents.Instance.TriggerStateChanged((int)triggerID);
    }

    public void PlayAnimation(string objectName, string animationName)
    {
        // Find the GameObject in the scene.
        GameObject obj = GameObject.Find(objectName);
        if (obj != null)
        {
            // Get the Animator component attached to the GameObject.
            Animator animator = obj.GetComponent<Animator>();
            if (animator != null)
            {
                // Play the specified animation.
                animator.Play(animationName);
            }
            else
            {
                Debug.LogError("Animator component not found on the object: " + objectName);
            }
        }
        else
        {
            Debug.LogError("Object not found: " + objectName);
        }
    }
}
