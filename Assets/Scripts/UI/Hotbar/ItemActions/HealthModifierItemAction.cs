using Opsive.Shared.Game;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.ItemActions;
using UnityEngine;

/// <summary>
/// Item action that modifies the user's health when used.
/// Reads a heal amount from the item's attributes and applies it.
/// </summary>
public class HealthModifierItemAction : ItemAction
{
    #region Serialized Fields
    /// <summary>
    /// Name of the attribute containing the heal amount.
    /// Must be a float attribute on the item.
    /// </summary>
    [SerializeField] private string m_AttributeName = "HealAmount";
    #endregion

    #region Protected Methods
    /// <summary>
    /// Checks if this action can be performed with the given item and user.
    /// </summary>
    /// <param name="itemInfo">Information about the item being used</param>
    /// <param name="itemUser">Entity trying to use the item</param>
    /// <returns>True if the action can be performed, false otherwise</returns>
    protected override bool CanInvokeInternal(ItemInfo itemInfo, ItemUser itemUser)
    {
        // Get the user's stats component
        var entity = itemUser.gameObject.GetCachedComponent<Entity>();
        var stats = entity.Core.GetCoreComponent<Stats>();

        // Verify stats component exists
        if (stats == null) return false;

        // Verify item has required attribute
        if (itemInfo.Item.GetAttribute<Attribute<float>>(m_AttributeName) == null) return false;

        return true;
    }

    /// <summary>
    /// Performs the health modification action.
    /// Applies the heal amount and removes the item from inventory.
    /// </summary>
    /// <param name="itemInfo">Information about the item being used</param>
    /// <param name="itemUser">Entity using the item</param>
    protected override void InvokeActionInternal(ItemInfo itemInfo, ItemUser itemUser)
    {
        // Get the user's stats
        var entity = itemUser.gameObject.GetCachedComponent<Entity>();
        var stats = entity.Core.GetCoreComponent<Stats>();

        // Apply the heal amount from item attributes
        float healAmount = itemInfo.Item.GetAttribute<Attribute<float>>(m_AttributeName).GetValue();
        stats.IncreaseHealth(healAmount);

        // Remove the used item
        itemInfo.Inventory.RemoveItem((1, itemInfo));
    }
    #endregion
}
