using PixelCrushers.DialogueSystem;
using System;
using UnityEngine;

public class CustomLua : MonoBehaviour
{
    void OnEnable()
    {
        Lua.RegisterFunction(nameof(CameraNewTarget), this, SymbolExtensions.GetMethodInfo(() => CameraNewTarget(string.Empty)));
        Lua.RegisterFunction(nameof(TriggerStateChanged), this, SymbolExtensions.GetMethodInfo(() => TriggerStateChanged(0)));
    }

    void OnDisable()
    {
        Lua.UnregisterFunction(nameof(CameraNewTarget)); // <-- Only if not on Dialogue Manager.
        Lua.UnregisterFunction(nameof(TriggerStateChanged));
    }

    private void CameraNewTarget(String cameraHookID) => GameEvents.Instance.CameraNewTarget(cameraHookID);
    private void TriggerStateChanged(double triggerID)
    {
        DialogueLua.SetVariable("Trigger." + triggerID, true);
        GameEvents.Instance.TriggerStateChanged((int)triggerID);
    }
}
