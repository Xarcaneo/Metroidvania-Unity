/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.DropsAndPickups
{
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using System;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    /// <summary>
    /// Simple item dropper with customizable drop rates.
    /// </summary>
    public class PercentageItemDropper : MonoBehaviour
    {
        [Tooltip("The list of possible items to drop with their chances")]
        [SerializeField] protected ItemDropChance[] m_PossibleDrops;

        [Tooltip("The pick up prefab, it must have a pick up component")]
        [SerializeField] protected GameObject m_PickUpPrefab;

        [Tooltip("The drop spawn point")]
        [SerializeField] protected Transform m_DropTransform;

        [Tooltip("The radius in which the drop can randomly spawn")]
        [SerializeField] protected float m_DropRadius = 1f;

        [Tooltip("Enable to show detailed debug logs about item drop chances and results")]
        [SerializeField] private bool m_ShowDebugLogs = false;

        [Serializable]
        public class ItemDropChance
        {
            [Tooltip("The item definition that can be dropped")]
            [SerializeField] protected ItemDefinition m_ItemDefinition;

            [Tooltip("The chance (0-100) that this item will drop")]
            [SerializeField][Range(0, 100)] protected float m_DropChance;

            public ItemDefinition ItemDefinition => m_ItemDefinition;
            public float DropChance => m_DropChance;
        }

        /// <summary>
        /// Get the array of possible drops for inspection.
        /// </summary>
        public ItemDropChance[] GetPossibleDrops()
        {
            return m_PossibleDrops;
        }

        /// <summary>
        /// Calculate the total drop chance of all items.
        /// </summary>
        public float GetTotalDropChance()
        {
            if (m_PossibleDrops == null) return 0f;
            
            float total = 0f;
            foreach (var drop in m_PossibleDrops)
            {
                if (drop.ItemDefinition != null)
                {
                    total += drop.DropChance;
                }
            }
            return total;
        }

        /// <summary>
        /// Drop items based on their configured chances.
        /// Only one item can be dropped at a time.
        /// </summary>
        public void Drop()
        {
            if (m_PossibleDrops == null || m_PossibleDrops.Length == 0)
            {
                if (m_ShowDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] No items configured to drop");
                }
                return;
            }

            float roll = UnityEngine.Random.Range(0f, 100f);
            float currentTotal = 0f;

            if (m_ShowDebugLogs)
            {
                Debug.Log($"[{gameObject.name}] Rolling for drop (Roll: {roll:F1}%)");
            }

            // Check each item in order until we find one that matches our roll
            foreach (var dropChance in m_PossibleDrops)
            {
                if (dropChance.ItemDefinition == null) continue;

                currentTotal += dropChance.DropChance;
                if (roll <= currentTotal)
                {
#if UNITY_EDITOR
                    // In editor, just show what would have dropped
                    if (!Application.isPlaying)
                    {
                        Debug.Log($"[{gameObject.name}] Would drop: {dropChance.ItemDefinition.name} (Testing in editor)");
                        return;
                    }
#endif

                    // Create the item
                    var item = InventorySystemManager.CreateItem(dropChance.ItemDefinition);
                    if (item == null) continue;

                    // Create the pickup at the drop position
                    var dropPosition = m_DropTransform != null ? m_DropTransform.position : transform.position;
                    var pickup = Instantiate(m_PickUpPrefab, dropPosition + GetDropOffset(), Quaternion.identity);
                    var itemObject = pickup.GetComponent<ItemObject>();
                    if (itemObject != null)
                    {
                        var itemCollection = new ItemCollection();
                        itemCollection.Initialize(null, true);
                        var itemInfo = new ItemInfo(new ItemAmount(item, 1), itemCollection, null);
                        itemObject.SetItem(itemInfo);
                        
                        if (m_ShowDebugLogs)
                        {
                            Debug.Log($"[{gameObject.name}] ✓ <color=green>SUCCESS</color> - Dropped {item.name}!");
                        }
                    }
                    return; // Exit after dropping one item
                }
            }

            if (m_ShowDebugLogs)
            {
                Debug.Log($"[{gameObject.name}] No item dropped (Roll too high)");
            }
        }

        /// <summary>
        /// Get a random offset for the drop position.
        /// </summary>
        protected virtual Vector3 GetDropOffset()
        {
            var circle = UnityEngine.Random.insideUnitCircle * m_DropRadius;
            return new Vector3(circle.x, 0, circle.y);
        }
    }
}
