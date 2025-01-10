using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages the upgrade levels and bonuses for character stats.
/// </summary>
[Serializable]
public class StatUpgradeLevel
{
    public string name;
    [Range(0, 500)]
    public float bonusPercentage;

    public StatUpgradeLevel(string name, float bonusPercentage)
    {
        this.name = name;
        this.bonusPercentage = bonusPercentage;
    }
}

public class StatUpgradeManager : CoreComponent
{
    [SerializeField]
    private List<StatUpgradeLevel> m_UpgradeLevels = new List<StatUpgradeLevel>
    {
        new StatUpgradeLevel("Normal", 0f),
        new StatUpgradeLevel("Enhanced", 25f),
        new StatUpgradeLevel("Advanced", 50f),
        new StatUpgradeLevel("Master", 100f)
    };

    [SerializeField]
    [Range(0, 3)]
    [Tooltip("Slide to select upgrade level (0: Normal, 1: Enhanced, 2: Advanced, 3: Master)")]
    private int m_CurrentUpgradeLevel = 0;

    [SerializeField]
    private UpgradeLevelIndicator m_Indicator;

    private void OnValidate()
    {
        // Ensure level is within valid range
        m_CurrentUpgradeLevel = Mathf.Clamp(m_CurrentUpgradeLevel, 0, m_UpgradeLevels.Count - 1);

        // Update UI in editor
        if (!Application.isPlaying)
        {
            // Try to find indicator if not assigned
            if (m_Indicator == null)
            {
                m_Indicator = GetComponentInChildren<UpgradeLevelIndicator>(true);
            }

            // Update indicator if found
            if (m_Indicator != null)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (m_Indicator != null)
                    {
                        m_Indicator.UpdateIndicators();
                        UnityEditor.SceneView.RepaintAll();
                    }
                };
            }
        }
    }

    /// <summary>
    /// Gets the current bonus multiplier based on the selected upgrade level
    /// </summary>
    public float CurrentBonusMultiplier
    {
        get
        {
            return 1f + (m_UpgradeLevels[m_CurrentUpgradeLevel].bonusPercentage / 100f);
        }
    }

    /// <summary>
    /// Sets the current upgrade level and updates stats
    /// </summary>
    public void SetUpgradeLevel(int level)
    {
        m_CurrentUpgradeLevel = Mathf.Clamp(level, 0, m_UpgradeLevels.Count - 1);
    }

    /// <summary>
    /// Gets the current upgrade level
    /// </summary>
    public int CurrentLevel => m_CurrentUpgradeLevel;

    /// <summary>
    /// Gets the current upgrade level name
    /// </summary>
    public string CurrentLevelName => m_UpgradeLevels[m_CurrentUpgradeLevel].name;
}
