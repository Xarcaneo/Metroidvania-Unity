using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    private static GameEvents _instance;

    public static GameEvents Instance { get => _instance; }

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
}