using PixelCrushers.DialogueSystem;
using UnityEngine;
using System.Collections;

/// <summary>
/// Core component responsible for managing player's souls.
/// Handles soul collection, storage, and persistence.
/// </summary>
public class SoulsManager : CoreComponent
{
    #region Constants
    /// <summary>
    /// Name of the souls variable in DialogueLua.
    /// </summary>
    private const string SOULS_VARIABLE = "Souls";
    #endregion

    #region Properties
    /// <summary>
    /// Current number of souls the player has.
    /// </summary>
    public int CurrentSouls { get; private set; }
    #endregion

    #region Events
    /// <summary>
    /// Event triggered when souls value changes. Passes the new souls value.
    /// </summary>
    public event System.Action<int> onSoulsValueChanged;
    #endregion

    #region Unity Lifecycle
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(LoadSoulsDelayed());
    }

    private void OnEnable()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnSoulsChanged += UpdateSouls;
        }
    }

    private void OnDisable()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnSoulsChanged -= UpdateSouls;
        }
    }

    private void OnDestroy()
    {
        // Clean up event
        onSoulsValueChanged = null;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Updates the player's soul count. Can be positive (gain souls) or negative (lose souls).
    /// </summary>
    /// <param name="amount">Amount of souls to add (positive) or remove (negative)</param>
    private void UpdateSouls(int amount)
    {
        // Prevent going below 0 souls
        if (CurrentSouls + amount < 0)
        {
            amount = -CurrentSouls; // Only remove what we have
        }

        CurrentSouls += amount;
        SaveSouls();
        onSoulsValueChanged?.Invoke(CurrentSouls);
    }

    /// <summary>
    /// Check if the player has enough souls.
    /// </summary>
    /// <param name="amount">Amount of souls to check for</param>
    /// <returns>True if player has enough souls, false otherwise</returns>
    public bool HasEnoughSouls(int amount)
    {
        return CurrentSouls >= amount;
    }
    #endregion

    #region Private Methods
    private IEnumerator LoadSoulsDelayed()
    {
        yield return new WaitForEndOfFrame();
        LoadSouls();
    }

    /// <summary>
    /// Load the current soul count from persistent storage.
    /// For new sessions, resets souls to 0.
    /// </summary>
    private void LoadSouls()
    {
        CurrentSouls = DialogueLua.GetVariable(SOULS_VARIABLE).asInt;

        // Trigger the event after loading the initial souls
        onSoulsValueChanged?.Invoke(CurrentSouls);
    }

    /// <summary>
    /// Save the current soul count to persistent storage.
    /// </summary>
    private void SaveSouls()
    {
        DialogueLua.SetVariable(SOULS_VARIABLE, CurrentSouls);
    }
    #endregion
}
