using System;
using UnityEngine;

/// <summary>
/// Manages game-wide events using the Singleton pattern.
/// Handles various game states, player actions, and system events.
/// </summary>
public class GameEvents : MonoBehaviour
{
    private static GameEvents _instance;

    /// <summary>
    /// Gets the singleton instance of GameEvents.
    /// </summary>
    public static GameEvents Instance => _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region Camera Events
    /// <summary>Event triggered when camera needs to focus on a new target.</summary>
    public event Action<string> onCameraNewTarget;
    public void CameraNewTarget(string cameraHookID) => onCameraNewTarget?.Invoke(cameraHookID);
    #endregion

    #region Game State Events
    /// <summary>Event triggered when game pause state changes.</summary>
    public event Action<bool> onPauseTrigger;
    public void PauseTrigger(bool isPaused) => onPauseTrigger?.Invoke(isPaused);

    /// <summary>Event triggered when dialogue state changes.</summary>
    public event Action<bool> onDialogueTrigger;
    public void DialogueTrigger(bool isDialogueActive) => onDialogueTrigger?.Invoke(isDialogueActive);

    /// <summary>Event triggered when UI visibility changes.</summary>
    public event Action<bool> onToggleUI;
    public void ToggleUI(bool state) => onToggleUI?.Invoke(state);
    #endregion

    #region Player Events
    /// <summary>Event triggered when player input should be disabled/enabled.</summary>
    public event Action<bool> onDeactivatePlayerInput;
    public void DeactivatePlayerInput(bool isChangingScene) => onDeactivatePlayerInput?.Invoke(isChangingScene);

    /// <summary>Event triggered when player interaction state changes.</summary>
    public event Action<bool> onPlayerInteractTrigger;
    public void InteractTrigger(bool isInteracting) => onPlayerInteractTrigger?.Invoke(isInteracting);

    /// <summary>Event triggered when player spawns.</summary>
    public event Action onPlayerSpawned;
    public void PlayerSpawned() => onPlayerSpawned?.Invoke();

    /// <summary>Event triggered when player dies.</summary>
    public event Action onPlayerDied;
    public void PlayerDied() => onPlayerDied?.Invoke();

    /// <summary>Event triggered when player position is set.</summary>
    public event Action onPlayerPositionSet;
    public void PlayerPositionSet() => onPlayerPositionSet?.Invoke();

    /// <summary>Event triggered when player state changes.</summary>
    public event Action<State> onPlayerStateChanged;
    public void PlayerStateChanged(State state) => onPlayerStateChanged?.Invoke(state);
    #endregion

    #region Level Events
    /// <summary>Event triggered when player enters a new room.</summary>
    public event Action<int> onRoomChanged;
    public void RoomChanged(int levelID) => onRoomChanged?.Invoke(levelID);

    /// <summary>Event triggered when a hidden room is discovered.</summary>
    public event Action onHiddenRoomRevealed;
    public void HiddenRoomRevealed() => onHiddenRoomRevealed?.Invoke();

    /// <summary>Event triggered when player enters a new area.</summary>
    public event Action<string> onAreaChanged;
    public void AreaChanged(string areaName) => onAreaChanged?.Invoke(areaName);
    #endregion

    #region Session Events
    /// <summary>Event triggered when a new game session starts.</summary>
    public event Action onNewSession;
    public void NewSession() => onNewSession?.Invoke();

    /// <summary>Event triggered when current session ends.</summary>
    public event Action onEndSession;
    public void EndSession() => onEndSession?.Invoke();

    /// <summary>Event triggered when game is being saved.</summary>
    public event Action onGameSaving;
    public void GameSaving() => onGameSaving?.Invoke();
    #endregion

    #region Gameplay Events
    /// <summary>Event triggered when a trigger's state changes.</summary>
    public event Action<string> onTriggerStateChanged;
    public void TriggerStateChanged(string triggerID) => onTriggerStateChanged?.Invoke(triggerID);

    /// <summary>Event triggered when a puzzle is opened.</summary>
    public event Action<string> onPuzzleOpen;
    public void PuzzleOpen(string puzzleName) => onPuzzleOpen?.Invoke(puzzleName);

    /// <summary>Event triggered when a puzzle is closed.</summary>
    public event Action<string> onPuzzleClose;
    public void PuzzleClose(string puzzleName) => onPuzzleClose?.Invoke(puzzleName);

    /// <summary>Event triggered when souls are collected.</summary>
    public event Action<int> onSoulsReceived;
    public void SoulsReceived(int soulsAmount) => onSoulsReceived?.Invoke(soulsAmount);
    #endregion
}