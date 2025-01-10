using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the visual display of entity upgrade levels using dot indicators
/// </summary>
public class UpgradeLevelIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StatUpgradeManager upgradeManager;
    [SerializeField] private Image[] levelDots;
    
    [Header("Settings")]
    [SerializeField] private Color upgradedDotColor = Color.yellow;
    [SerializeField] private Color defaultDotColor = Color.black;

    private void Start()
    {
        UpdateIndicators();
    }

#if UNITY_EDITOR
    /// <summary>
    /// Updates the dots in the Unity Editor when values change
    /// </summary>
    private void OnValidate()
    {
        // Only update in editor mode
        if (!Application.isPlaying)
        {
            UpdateIndicators();
        }
    }
#endif

    /// <summary>
    /// Updates the upgrade level indicator dots based on the current upgrade level
    /// </summary>
    public void UpdateIndicators()
    {
        if (upgradeManager == null || levelDots == null) return;

        int currentLevel = upgradeManager.CurrentLevel;

        for (int i = 0; i < levelDots.Length; i++)
        {
            if (levelDots[i] != null)
            {
                // Only color dots if level is above 0 (i.e., Enhanced or higher)
                levelDots[i].color = currentLevel > 0 && i < currentLevel ? upgradedDotColor : defaultDotColor;
            }
        }
    }
}
