using UnityEngine;
using System;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.DropsAndPickups;

[Serializable]
public class LocationDropTable
{
    [Tooltip("The locations where this drop table applies")]
    [SerializeField] private LocationName[] m_Locations;
    
    [Tooltip("The list of possible items to drop with their chances for these locations")]
    [SerializeField] private PercentageItemDropper.ItemDropChance[] m_PossibleDrops;

    public LocationName[] Locations => m_Locations;
    public PercentageItemDropper.ItemDropChance[] PossibleDrops => m_PossibleDrops;
}

public class LocationBasedItemDropper : PercentageItemDropper
{
    [Tooltip("List of drop tables for different locations")]
    [SerializeField] private LocationDropTable[] m_LocationDropTables;

    [Tooltip("Which drop table to preview in editor (0-based index)")]
    [SerializeField] [Range(0, 10)] private int m_DebugTableIndex;

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
                m_PossibleDrops = m_LocationDropTables[m_DebugTableIndex].PossibleDrops;
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
                    // Update the base class drop table
                    m_PossibleDrops = dropTable.PossibleDrops;
                    Debug.Log($"Using drop table {i} for location {currentLocation}");
                    return;
                }
            }
        }
        Debug.Log($"No matching drop table found for location {currentLocation}");
    }
}
