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

    /// <summary>
    /// Event triggered when the player menu is opened or closed.
    /// </summary>
    public event Action onPlayerMenuOpen;
    public void PlayerMenuOpen() => onPlayerMenuOpen?.Invoke();
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

    /// <summary>Event triggered when essence is spawned or collected in a sroom.</summary>
    public event Action<int,bool> onRoomEssenceChanged;
    public void RoomEssenceChanged(int levelID, bool spawned) => onRoomEssenceChanged?.Invoke(levelID, spawned);

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

    /// Event triggered when the soul count changes (can be positive or negative).
    public event Action<int> OnSoulsChanged;
    public void SoulsChanged(int soulsAmount) => OnSoulsChanged?.Invoke(soulsAmount);

    /// <summary>Event triggered when player essence needs to be spawned</summary>
    public event Action<Vector2, int> onPlayerEssenceSpawn;
    public void PlayerEssenceSpawn(Vector2 position, int soulsAmount) => onPlayerEssenceSpawn?.Invoke(position, soulsAmount);

    /// <summary>Event triggered when a player essence is collected.</summary>
    public event Action<PlayerEssence> onEssenceCollected;
    public void EssenceCollected(PlayerEssence essence) => onEssenceCollected?.Invoke(essence);

    /// <summary>Event triggered when the game language changes</summary>
    public event Action onLanguageChanged;

    /// <summary>Notifies subscribers that the game language has changed</summary>
    public void OnLanguageChanged() => onLanguageChanged?.Invoke();
    #endregion
}