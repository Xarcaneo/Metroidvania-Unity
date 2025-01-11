/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.DropsAndPickups
{
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Percentage based Item dropper. Uses the item amounts in inventory as percentage chances.
    /// </summary>
    public class PercentageItemDropper : ItemDropper
    {
        [Tooltip("Enable to show detailed debug logs about item drop chances and results")]
        [SerializeField] private bool m_ShowDebugLogs = false;

        protected List<ItemInfo> m_SelectedItems;

        /// <summary>
        /// Initialize the component.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            m_SelectedItems = new List<ItemInfo>();
        }

        /// <summary>
        /// Get the items to drop based on percentage chances.
        /// </summary>
        /// <returns>Returns a list of items to drop.</returns>
        protected override ListSlice<ItemInfo> GetItemsToDropInternal()
        {
            if (m_Inventory == null) { return new ListSlice<ItemInfo>(); }

            var itemCollection = m_Inventory.GetItemCollection(m_ItemCollectionID);
            if (itemCollection == null) { return new ListSlice<ItemInfo>(); }

            m_SelectedItems.Clear();
            var allItems = itemCollection.GetAllItemStacks();

            // Check each item against its percentage chance
            for (int i = 0; i < allItems.Count; i++) {
                var itemInfo = allItems[i];
                if (itemInfo.Item == null || itemInfo.Amount <= 0) { continue; }

                // Use amount as percentage (0-100)
                float dropChance = Mathf.Clamp(itemInfo.Amount, 0, 100);
                
                // Random roll for this item
                float randomRoll = Random.Range(0f, 100f);
                if (m_ShowDebugLogs) {
                    Debug.Log($"[{gameObject.name} ({transform.root.name})] Rolling for {itemInfo.Item.name} (Drop chance: {dropChance}%)");
                }
                
                if (randomRoll < dropChance) {
                    // If successful, add one of this item to drop
                    m_SelectedItems.Add(new ItemInfo(1, itemInfo.Item));
                    if (m_ShowDebugLogs) {
                        Debug.Log($"[{gameObject.name}] ✓ <color=green>SUCCESS</color> - {itemInfo.Item.name} will drop! (Rolled {randomRoll:F1} < {dropChance}%)");
                    }
                } else {
                    if (m_ShowDebugLogs) {
                        Debug.Log($"[{gameObject.name}] ✗ <color=red>FAILED</color> - {itemInfo.Item.name} will not drop (Rolled {randomRoll:F1} > {dropChance}%)");
                    }
                }
            }

            // Log summary of what will drop
            if (m_ShowDebugLogs) {
                if (m_SelectedItems.Count > 0) {
                    string dropList = string.Join(", ", m_SelectedItems.Select(item => item.Item.name));
                    Debug.Log($"[{gameObject.name}] 📦 <color=yellow>Items dropping: {dropList}</color>");
                } else {
                    Debug.Log($"[{gameObject.name}] 🚫 <color=red>No items will drop this time</color>");
                }
            }

            return m_SelectedItems.ToListSlice();
        }
    }
}
