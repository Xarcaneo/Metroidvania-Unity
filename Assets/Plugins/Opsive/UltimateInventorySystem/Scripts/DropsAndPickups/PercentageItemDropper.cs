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
        [SerializeField] protected ItemDropChance[] m_PossibleDrops = new ItemDropChance[0];

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

            /// <summary>
            /// Constructor for creating a new drop chance entry.
            /// </summary>
            public ItemDropChance()
            {
                m_ItemDefinition = null;
                m_DropChance = 0f;
            }

            /// <summary>
            /// Constructor with initial values.
            /// </summary>
            public ItemDropChance(ItemDefinition itemDefinition, float dropChance)
            {
                m_ItemDefinition = itemDefinition;
                m_DropChance = Mathf.Clamp(dropChance, 0f, 100f);
            }
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
            // Validate pickup prefab (only in play mode)
            if (Application.isPlaying)
            {
                if (m_PickUpPrefab == null)
                {
                    Debug.LogError($"[{gameObject.name}] PickUpPrefab is not assigned!");
                    return;
                }

                if (m_PickUpPrefab.GetComponent<ItemObject>() == null)
                {
                    Debug.LogError($"[{gameObject.name}] PickUpPrefab must have an ItemObject component!");
                    return;
                }
            }

            // Check for empty drops array
            if (m_PossibleDrops == null || m_PossibleDrops.Length == 0)
            {
                Debug.LogWarning($"[{gameObject.name}] No items configured to drop");
                return;
            }

            // Calculate total drop chance
            float totalChance = GetTotalDropChance();
            if (totalChance <= 0f)
            {
                Debug.LogWarning($"[{gameObject.name}] Total drop chance is 0% - nothing will drop");
                return;
            }

            // Roll between 0-100% to determine if and what drops
            float roll = UnityEngine.Random.Range(0f, 100f);
            float currentTotal = 0f;

            // Check each item in order until we find one that matches our roll
            foreach (var dropChance in m_PossibleDrops)
            {
                if (dropChance.ItemDefinition == null) continue;
                if (dropChance.DropChance <= 0f) continue;

                currentTotal += dropChance.DropChance;
                if (roll <= currentTotal)
                {
#if UNITY_EDITOR
                    // In editor, just show what would have dropped
                    if (!Application.isPlaying)
                    {
                        Debug.Log($"[{gameObject.name}] Would drop: {dropChance.ItemDefinition.name} (Roll: {roll:F2}% ≤ {currentTotal:F2}%)");
                        return;
                    }
#endif

                    // Create the item
                    var item = InventorySystemManager.CreateItem(dropChance.ItemDefinition);
                    if (item == null)
                    {
                        Debug.LogError($"[{gameObject.name}] Failed to create item from definition: {dropChance.ItemDefinition.name}");
                        continue;
                    }

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
                            Debug.Log($"[{gameObject.name}] ✓ <color=green>SUCCESS</color> - Dropped {item.name}! (Roll: {roll:F2}% ≤ {currentTotal:F2}%)");
                        }
                    }
                    return; // Exit after dropping one item
                }
            }

            // Nothing dropped - roll was too high
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.Log($"[{gameObject.name}] No drop - Roll too high (Roll: {roll:F2}% > Total Chance: {totalChance:F2}%)");
                return;
            }
#endif

            if (m_ShowDebugLogs)
            {
                Debug.Log($"[{gameObject.name}] ✗ <color=red>NO DROP</color> - Roll too high (Roll: {roll:F2}% > Total Chance: {totalChance:F2}%)");
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Validate the pickup prefab has the required component
            if (m_PickUpPrefab != null && m_PickUpPrefab.GetComponent<ItemObject>() == null)
            {
                Debug.LogWarning($"[{gameObject.name}] PickUpPrefab must have an ItemObject component!");
            }

            // Check for any items with 0% drop chance
            if (m_PossibleDrops != null)
            {
                foreach (var drop in m_PossibleDrops)
                {
                    if (drop.ItemDefinition != null && drop.DropChance <= 0f)
                    {
                        Debug.LogWarning($"[{gameObject.name}] Item {drop.ItemDefinition.name} has 0% drop chance");
                    }
                }
            }
        }
#endif
    }
}
