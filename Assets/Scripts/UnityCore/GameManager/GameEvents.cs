using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    private static GameEvents _instance;

    public static GameEvents Instance { get => _instance; }

    void OnEnable()
    {
        Lua.RegisterFunction(nameof(CameraNewTarget), this, SymbolExtensions.GetMethodInfo(() => CameraNewTarget(string.Empty)));
    }

    void OnDisable()
    {
        Lua.UnregisterFunction(nameof(CameraNewTarget)); // <-- Only if not on Dialogue Manager.
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public event Action<String> onCameraNewTarget;
    public void CameraNewTarget(String cameraHookID)
    {
        if (onCameraNewTarget != null)
        {
            onCameraNewTarget(cameraHookID);
        }
    }

    public event Action<bool> onPauseTrigger;
    public void PauseTrigger(bool isPaused)
    {
        if(onPauseTrigger != null)
        {
            onPauseTrigger(isPaused);
        }
    }

    public event Action<bool> onDialogueTrigger;
    public void DialogueTrigger(bool isDialogueActive)
    {
        if (onDialogueTrigger != null)
        {
            onDialogueTrigger(isDialogueActive);
        }
    }

    public event Action<bool> onDeactivatePlayerInput;
    public void DeactivatePlayerInput(bool isChangingScene)
    {
        if (onDeactivatePlayerInput != null)
        {
            onDeactivatePlayerInput(isChangingScene);
        }
    }

    public event Action<bool> onPlayerInteractTrigger;
    public void InteractTrigger(bool isInteracting)
    {
        if (onPlayerInteractTrigger != null)
        {
            onPlayerInteractTrigger(isInteracting);
        }
    }

    public event Action onPlayerSpawned;
    public void PlayerSpawned()
    {
        if (onPlayerSpawned != null)
        {
            onPlayerSpawned();
        }
    }

    public event Action onPlayerDied;
    public void PlayerDied()
    {
        if (onPlayerDied != null)
        {
            onPlayerDied();
        }
    }

    public event Action onPlayerPositionSet;
    public void PlayerPositionSet()
    {
        if (onPlayerPositionSet != null)
        {
            onPlayerPositionSet();
        }
    }

    public event Action<State> onPlayerStateChanged;
    public void PlayerStateChanged(State state)
    {
        if (onPlayerStateChanged != null)
        {
            onPlayerStateChanged(state);
        }
    }

    public event Action<int> onRoomChanged;
    public void RoomChanged(int levelID)
    {
        if (onRoomChanged != null)
        {
            onRoomChanged(levelID);
        }
    }

    public event Action<string> onAreaChanged;
    public void AreaChanged(string areaName)
    {
        if (onAreaChanged != null)
        {
            onAreaChanged(areaName);
        }
    }

    public event Action onNewSession;
    public void NewSession()
    {
        if (onNewSession != null)
        {
            onNewSession();
        }
    }

    public event Action onEndSession;
    public void EndSession()
    {
        if (onEndSession != null)
        {
            onEndSession();
        }
    }

    public event Action<int> onTriggerStateChanged;
    public void TriggerStateChanged(int triggerID)
    {
        if (onTriggerStateChanged != null)
        {
            onTriggerStateChanged(triggerID);
        }
    }

    public event Action onGameSaving;
    public void GameSaving()
    {
        if (onGameSaving != null)
        {
            onGameSaving();
        }
    }

    public event Action<bool> onToggleUI;
    public void ToggleUI(bool state)
    {
        if (onToggleUI != null)
        {
            onToggleUI(state);
        }
    }
}