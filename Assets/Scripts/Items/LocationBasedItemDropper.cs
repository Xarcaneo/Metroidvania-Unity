using UnityEngine;
using System;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.DropsAndPickups;
using Opsive.Shared.Game;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;

[Serializable]
public class LocationDropTable
{
    [Tooltip("The locations where this drop table applies")]
    [SerializeField] private LocationName[] m_Locations;
    
    [Tooltip("The list of possible items to drop with their chances for these locations")]
    [SerializeField] private ItemDropChance[] m_PossibleDrops;

    public LocationName[] Locations => m_Locations;
    public ItemDropChance[] PossibleDrops => m_PossibleDrops;
}

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

public class LocationBasedItemDropper : Dropper
{
    [Tooltip("List of drop tables for different locations")]
    [SerializeField] private LocationDropTable[] m_LocationDropTables;

    [Tooltip("Which drop table to preview in editor (0-based index)")]
    [SerializeField] [Range(0, 10)] private int m_DebugTableIndex;

    [Tooltip("Enable to show detailed debug logs about item drop chances and results")]
    [SerializeField] private bool m_ShowDebugLogs = false;

    [Tooltip("The current drop table being used")]
    private ItemDropChance[] m_CurrentDrops;


    private void Start()
    {
        // Get the current location from the active LevelManager
        var levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            UpdateDropTableForLocation(levelManager.AreaName);
        }
    }

    private void OnValidate()
    {
        // Clamp debug index to valid range
        if (m_LocationDropTables != null && m_LocationDropTables.Length > 0)
        {
            m_DebugTableIndex = Mathf.Clamp(m_DebugTableIndex, 0, m_LocationDropTables.Length - 1);
            if (m_LocationDropTables[m_DebugTableIndex] != null)
            {
                m_CurrentDrops = m_LocationDropTables[m_DebugTableIndex].PossibleDrops;
            }
        }
    }

    private void UpdateDropTableForLocation(LocationName currentLocation)
    {
        // Look for a matching drop table
        for (int i = 0; i < m_LocationDropTables.Length; i++)
        {
            var dropTable = m_LocationDropTables[i];
            foreach (var location in dropTable.Locations)
            {
                if (location == currentLocation)
                {
                    // Update the current drop table
                    m_CurrentDrops = dropTable.PossibleDrops;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Calculate the total drop chance of all items.
    /// </summary>
    public float GetTotalDropChance()
    {
        if (m_CurrentDrops == null) return 0f;
        
        float total = 0f;
        foreach (var drop in m_CurrentDrops)
        {
            if (drop.ItemDefinition != null)
            {
                total += drop.DropChance;
            }
        }
        return total;
    }

    public override void Drop()
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
        if (m_CurrentDrops == null || m_CurrentDrops.Length == 0)
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
        foreach (var dropChance in m_CurrentDrops)
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

                // Create the pickup at the drop position using object pooling
                var position = m_DropTransform != null ? m_DropTransform.position : transform.position;
                var dropPosition = position + DropOffset();
                var pickupGameObject = ObjectPool.Instantiate(m_PickUpPrefab, dropPosition, Quaternion.identity);
                var itemObject = pickupGameObject.GetComponent<ItemObject>();

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

    #if UNITY_EDITOR
    /// <summary>
    /// Get all drop tables for editor debugging.
    /// Only available in editor.
    /// </summary>
    public LocationDropTable[] GetLocationDropTables()
    {
        return m_LocationDropTables;
    }

    /// <summary>
    /// Get the current drop table.
    /// Only available in editor.
    /// </summary>
    public ItemDropChance[] GetPossibleDrops()
    {
        return m_CurrentDrops;
    }
    #endif
}
