using Opsive.UltimateInventorySystem.UI.Panels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTab : Tab
{
    [SerializeField] DisplayPanel displayPanel;

    private void OnEnable() => displayPanel.SmartOpen();
    private void OnDisable() => displayPanel.SmartClose();
}
