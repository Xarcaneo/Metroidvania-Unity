using UnityEngine;
using UnityEditor;
using Opsive.UltimateInventorySystem.Editor.Inspectors;
using System.Text;

[CustomEditor(typeof(LocationBasedItemDropper))]
public class LocationBasedItemDropperInspector : PercentageItemDropperInspector
{
    protected LocationBasedItemDropper m_LocationBasedItemDropper;
    protected SerializedProperty m_LocationDropTables;
    protected SerializedProperty m_DebugTableIndex;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_LocationBasedItemDropper = target as LocationBasedItemDropper;
        m_LocationDropTables = serializedObject.FindProperty("m_LocationDropTables");
        m_DebugTableIndex = serializedObject.FindProperty("m_DebugTableIndex");
    }

    /// <summary>
    /// Format a percentage value with appropriate precision.
    /// Shows more decimal places for very small numbers.
    /// </summary>
    private string FormatPercentage(float value)
    {
        if (value < 0.01f && value > 0f)
            return value.ToString("F4"); // Show 4 decimal places for very small numbers
        else if (value < 0.1f)
            return value.ToString("F3"); // Show 3 decimal places for small numbers
        else
            return value.ToString("F2"); // Show 2 decimal places normally
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw location drop tables first
        EditorGUILayout.PropertyField(m_LocationDropTables);

        // Create a slider that goes from 0 to the number of tables - 1
        int maxIndex = Mathf.Max(0, m_LocationDropTables.arraySize - 1);
        m_DebugTableIndex.intValue = EditorGUILayout.IntSlider(
            new GUIContent("Debug Table Index", "Which drop table to preview in editor"),
            m_DebugTableIndex.intValue, 
            0, 
            maxIndex
        );

        EditorGUILayout.Space();

        // Draw all base properties except our custom ones
        DrawPropertiesExcluding(serializedObject, new string[] { "m_LocationDropTables", "m_DebugTableIndex", "m_Script" });

        EditorGUILayout.Space();

        // Add debug buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Debug Drop Chances", GUILayout.Height(30)))
        {
            DebugDropChances();
        }
        if (GUILayout.Button("Test Drop", GUILayout.Height(30)))
        {
            m_LocationBasedItemDropper.Drop();
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    private void DebugDropChances()
    {
        var dropChances = m_LocationBasedItemDropper.GetPossibleDrops();
        if (dropChances == null || dropChances.Length == 0)
        {
            Debug.LogWarning("No items configured to drop!");
            return;
        }

        var sb = new StringBuilder();
        float totalChance = m_LocationBasedItemDropper.GetTotalDropChance();
        
        // Header
        sb.AppendLine($"\n<color=#00FF00>=== Drop Table Analysis for {m_LocationBasedItemDropper.gameObject.name} ===</color>");
        sb.AppendLine($"<color=#87CEEB>Total Items: {dropChances.Length}</color>");
        
        // Warning if total chance exceeds 100%
        if (totalChance > 100f)
        {
            sb.AppendLine($"\n<color=#FF0000>⚠ WARNING: Total drop chance ({FormatPercentage(totalChance)}%) exceeds 100%!</color>");
            sb.AppendLine("<color=#FF0000>Only the first item that matches the roll will be dropped.</color>\n");
        }
        else
        {
            float noDropChance = 100f - totalChance;
            sb.AppendLine($"<color=#87CEEB>No Drop Chance: {FormatPercentage(noDropChance)}%</color>\n");
        }

        // Show drop chances for each item
        sb.AppendLine("<color=#FFD700>Item Drop Chances (in order):</color>");
        float currentTotal = 0f;
        foreach (var dropChance in dropChances)
        {
            if (dropChance.ItemDefinition == null) continue;
            
            float rangeStart = currentTotal;
            currentTotal += dropChance.DropChance;
            
            sb.AppendLine($"<color=#FFA500>• {dropChance.ItemDefinition.name}:</color>");
            sb.AppendLine($"  <color=#98FB98>Drop Chance: {FormatPercentage(dropChance.DropChance)}%</color>");
            sb.AppendLine($"  <color=#98FB98>Roll Range: {FormatPercentage(rangeStart)}% - {FormatPercentage(currentTotal)}%</color>");
        }

        sb.AppendLine("\n<color=#87CEEB>Note: A single roll (0-100%) determines which item drops.</color>");
        sb.AppendLine("<color=#87CEEB>The first item whose range contains the roll will be dropped.</color>");
        sb.AppendLine("<color=#87CEEB>Rolls use floating point precision for accurate probabilities.</color>");
        
        Debug.Log(sb.ToString());
    }
}
