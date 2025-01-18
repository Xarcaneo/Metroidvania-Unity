using PixelCrushers;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles player-specific death behavior and respawn mechanics.
/// </summary>
public class PlayerDeath : Death
{
    #region Raycast Settings

    /// <summary>
    /// The layer mask for solid objects.
    /// </summary>
    [SerializeField] private LayerMask solidLayer;

    /// <summary>
    /// The offset from the player's position for the left raycast.
    /// </summary>
    [SerializeField] private Vector3 downffsetLeft = new Vector3(-2, 0f, 0);

    /// <summary>
    /// The offset from the player's position for the right raycast.
    /// </summary>
    [SerializeField] private Vector3 downffsetRight = new Vector3(2, 0f, 0);

    /// <summary>
    /// The offset from the player's position for the bottom raycast.
    /// </summary>
    [SerializeField] private Vector3 bottomOffset = new Vector3(0, -1.5f, 0);

    /// <summary>
    /// The offset from the player's position for the top raycast.
    /// </summary>
    [SerializeField] private Vector3 topOffset = new Vector3(0, 0.5f, 0);

    #endregion

    #region Dependencies

    /// <summary>
    /// Reference to the Movement component.
    /// </summary>
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    /// <summary>
    /// Reference to the SoulsManager component.
    /// </summary>
    private SoulsManager SoulsManager { get => soulsManager ?? core.GetCoreComponent(ref soulsManager); }
    private SoulsManager soulsManager;

    /// <summary>
    /// Stores the room number of the last safe position.
    /// </summary>
    public int LastSafeRoomNumber { get; private set; }

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        if (Movement != null)
        {
            Movement.OnLastSafePositionUpdated += HandleLastSafePositionUpdated;
        }
    }

    private void OnDisable()
    {
        if (Movement != null)
        {
            Movement.OnLastSafePositionUpdated -= HandleLastSafePositionUpdated;
        }
    }

    #endregion

    #region Death Implementation

    /// <summary>
    /// Implements player-specific death behavior.
    /// </summary>
    public override void Die()
    {
        base.Die();

        // Trigger essence spawn event with last safe position and current souls amount
        GameEvents.Instance.PlayerEssenceSpawn(Movement.LastSafePosition,LastSafeRoomNumber, SoulsManager.CurrentSouls);

        int active_slot = GameManager.Instance.currentSaveSlot;
        StartCoroutine(SaveAfterFrame(active_slot));
    }

    /// <summary>
    /// Updates the player's last safe position and its corresponding room number.
    /// </summary>
    /// <param name="position">The new last safe position.</param>
    public void HandleLastSafePositionUpdated(Vector2 position)
    {
        if (AreaManager.Instance?.ActiveArea != null)
        {
            LastSafeRoomNumber = AreaManager.Instance.ActiveArea.roomNumber;
        }
    }

    #endregion

    #region Private Methods

    private IEnumerator SaveAfterFrame(int slot)
    {
        yield return new WaitForEndOfFrame();
        SaveSystem.SaveToSlot(slot);
    }

    #endregion
}
