using Opsive.UltimateInventorySystem.Core;
using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles lock functionality for key-based interactions.
/// Manages lock state persistence, key validation, and visual feedback.
/// Integrates with the Ultimate Inventory System for item checking.
/// </summary>
public class Lock : InteractableState
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Item definition required to unlock")]
    /// <summary>
    /// Item definition required to unlock this lock.
    /// Must match an item definition in the Ultimate Inventory System.
    /// </summary>
    private ItemDefinition m_itemDefinition;
    #endregion

    #region Private Fields
    // Animation state names
    /// <summary>
    /// Constants for animation state names to ensure consistency
    /// and prevent typos in animation calls.
    /// </summary>
    private const string NO_KEY_ANIM = "NoKeyAnimation";
    private const string UNLOCKED_IDLE_ANIM = "Idle";
    #endregion

    #region Unity Lifecycle
    // Start is now handled by base class
    #endregion

    #region Public Methods
    /// <summary>
    /// Handles interaction with the lock when activated by the player.
    /// Checks for required key item and updates lock state accordingly.
    /// </summary>
    public override void Interact()
    {
        var item = InventorySystemManager.CreateItem(m_itemDefinition);
        if (item == null)
        {
            Debug.LogError($"[{gameObject.name}] Failed to create item: {m_itemDefinition}");
            return;
        }

        var inventory = Player.Instance.m_inventory;
        if (inventory.HasItem(item, false))
        {
            // Get the actual item from inventory before removing
            var itemInfo = inventory.GetItemInfo(m_itemDefinition);
            if (itemInfo.HasValue)
            {
                // Remove the key item from inventory using the actual item instance
                inventory.RemoveItem(itemInfo.Value.Item, 1);
                UnlockAndNotify();
            }
        }
        else
        {
            m_animator.Play(NO_KEY_ANIM);
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Unlocks the lock and notifies connected gates
    /// </summary>
    private void UnlockAndNotify()
    {
        canInteract = false; // Disable further interaction
        UpdateState(true);
    }

    /// <summary>
    /// Called when the lock's state changes
    /// </summary>
    /// <param name="newState">The new state value</param>
    protected override void OnStateChanged(bool newState)
    {
        if (newState)
        {
            m_animator.Play(UNLOCKED_IDLE_ANIM);
        }
    }
    #endregion
}
