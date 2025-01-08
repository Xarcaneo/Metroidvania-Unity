using UnityEngine;
using System;

/// <summary>
/// Detects and manages interactions with items in the game world.
/// Provides events for when items enter and exit the detection area.
/// </summary>
public class ItemDetector : CoreComponent
{
    #region Events

    /// <summary>
    /// Event triggered when an item is detected or leaves detection range.
    /// </summary>
    public event Action<bool> onItemDetected;

    #endregion

    #region Properties

    /// <summary>
    /// Number of items currently detected in the trigger area.
    /// </summary>
    public int itemsDetected { get; private set; }

    #endregion

    #region Unity Callbacks

    /// <summary>
    /// Initializes the detector and subscribes to game events.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        try
        {
            GameEvents.Instance.onPauseTrigger += EnableDisableComponent;
            GameEvents.Instance.onDialogueTrigger += EnableDisableComponent;
        }
        catch
        {
            // Silently handle case where GameEvents is not initialized
        }
    }

    /// <summary>
    /// Unsubscribes from game events when the component is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        try
        {
            GameEvents.Instance.onPauseTrigger -= EnableDisableComponent;
            GameEvents.Instance.onDialogueTrigger -= EnableDisableComponent;
        }
        catch
        {
            // Silently handle case where GameEvents is not initialized
        }
    }

    /// <summary>
    /// Enables or disables the component based on game state.
    /// </summary>
    /// <param name="state">True to disable, false to enable</param>
    public override void EnableDisableComponent(bool state)
    {
        base.EnableDisableComponent(!state);
    }

    /// <summary>
    /// Called when another collider enters the trigger area.
    /// </summary>
    /// <param name="collision">The collider that entered the trigger</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            itemsDetected++;
            onItemDetected?.Invoke(true);
        }
    }

    /// <summary>
    /// Called when another collider exits the trigger area.
    /// </summary>
    /// <param name="collision">The collider that exited the trigger</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            itemsDetected--;
            if (itemsDetected <= 0)
            {
                itemsDetected = 0;
                onItemDetected?.Invoke(false);
            }
        }
    }

    #endregion
}
