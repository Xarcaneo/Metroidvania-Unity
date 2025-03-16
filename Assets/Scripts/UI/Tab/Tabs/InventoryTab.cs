using Opsive.UltimateInventorySystem.UI.Panels;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Manages the inventory tab in the UI, handling the display and updates of the player's inventory.
/// </summary>
public class InventoryTab : Tab
{
    [Tooltip("Reference to the Ultimate Inventory System display panel")]
    [SerializeField] private DisplayPanel displayPanel;

    /// <summary>
    /// Opens the inventory display when the tab is enabled.
    /// </summary>
    private void OnEnable() => displayPanel.SmartOpen();

    /// <summary>
    /// Closes the inventory display when the tab is disabled.
    /// </summary>
    private void OnDisable()
    {
        if (displayPanel != null)
        {
            displayPanel.SmartClose();
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
