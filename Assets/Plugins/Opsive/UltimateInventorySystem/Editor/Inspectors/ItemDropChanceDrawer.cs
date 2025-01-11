/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.Inspectors
{
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.DropsAndPickups;
    using UnityEngine;
    using UnityEditor;
    using System.Reflection;

    /// <summary>
    /// Custom property drawer for ItemDropChance to make it look like the item list.
    /// </summary>
    [CustomPropertyDrawer(typeof(PercentageItemDropper.ItemDropChance))]
    public class ItemDropChanceDrawer : PropertyDrawer
    {
        private const float SPACING = 2f;
        private const float ICON_SIZE = 20f;
        private const float CHANCE_WIDTH = 60f;

        // Cache the icon field info
        private static FieldInfo m_EditorIconField = typeof(ItemDefinition).GetField("m_EditorIcon", 
            BindingFlags.Instance | BindingFlags.NonPublic);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var itemDefinitionProp = property.FindPropertyRelative("m_ItemDefinition");
            var dropChanceProp = property.FindPropertyRelative("m_DropChance");

            // Background
            var bgColor = EditorGUIUtility.isProSkin ? new Color(0.3f, 0.3f, 0.3f) : new Color(0.8f, 0.8f, 0.8f);
            EditorGUI.DrawRect(position, bgColor);

            position.x += SPACING;
            position.width -= SPACING * 2;

            // Draw item icon or default icon
            var iconRect = new Rect(position.x, position.y, ICON_SIZE, ICON_SIZE);
            var itemDef = itemDefinitionProp.objectReferenceValue as ItemDefinition;
            if (itemDef != null && m_EditorIconField != null)
            {
                var icon = m_EditorIconField.GetValue(itemDef) as Sprite;
                if (icon != null)
                {
                    GUI.DrawTexture(iconRect, icon.texture);
                }
                else
                {
                    EditorGUI.DrawRect(iconRect, Color.gray);
                }
            }
            else
            {
                EditorGUI.DrawRect(iconRect, Color.gray);
            }

            // Item Definition field
            var itemRect = new Rect(position.x + ICON_SIZE + SPACING, position.y, 
                                  position.width - ICON_SIZE - CHANCE_WIDTH - SPACING * 3, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(itemRect, itemDefinitionProp, GUIContent.none);

            // Drop chance field with % symbol
            var chanceRect = new Rect(position.x + position.width - CHANCE_WIDTH, position.y,
                                    CHANCE_WIDTH - SPACING, EditorGUIUtility.singleLineHeight);
            dropChanceProp.floatValue = EditorGUI.FloatField(chanceRect, dropChanceProp.floatValue);
            var percentRect = new Rect(chanceRect.x + chanceRect.width - 15, chanceRect.y, 15, chanceRect.height);
            EditorGUI.LabelField(percentRect, "%");

            EditorGUI.EndProperty();
        }
    }
}
