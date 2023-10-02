using Opsive.Shared.Game;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.ItemActions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthModifierItemAction : ItemAction
{
    [SerializeField] private string m_AttributeName = "HealAmount";

    protected override bool CanInvokeInternal(ItemInfo itemInfo, ItemUser itemUser)
    {
        var entity = itemUser.gameObject.GetCachedComponent<Entity>();
        var stats = entity.Core.GetCoreComponent<Stats>();

        if (stats == null) return false;

        if (itemInfo.Item.GetAttribute<Attribute<float>>(m_AttributeName) == null) return false;

        return true;
    }

    protected override void InvokeActionInternal(ItemInfo itemInfo, ItemUser itemUser)
    {
        var entity = itemUser.gameObject.GetCachedComponent<Entity>();
        var stats = entity.Core.GetCoreComponent<Stats>();

        stats.IncreaseHealth(itemInfo.Item.GetAttribute<Attribute<float>>(m_AttributeName).GetValue());

        itemInfo.Inventory.RemoveItem((1, itemInfo));
    }
}
