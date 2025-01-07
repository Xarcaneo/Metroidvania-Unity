/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Demo.UI.VisualStructures.AttributeUIs
{
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Equipping;
    using Opsive.UltimateInventorySystem.UI.Item.AttributeViewModules;
    using UnityEngine;
    using UnityEngine.UI;
    using Text = Opsive.Shared.UI.Text;

    /// <summary>
    /// Compares an attribute value to the matching character equipper item attribute value.
    /// Visualizes the comparison between current and potential equipment stats.
    /// </summary>
    public class CompareAttributeView : AttributeViewModule
    {
        #region Serialized Fields
        [Tooltip("The stat name to compare")]
        [SerializeField] protected string m_StatName;

        [Tooltip("Text component displaying the current value")]
        [SerializeField] protected Text m_CurrentValueText;

        [Tooltip("Text component displaying the potential new value")]
        [SerializeField] protected Text m_NewValueText;

        [Tooltip("Arrow image that changes color based on value comparison")]
        [SerializeField] protected Image m_ArrowImage;
        #endregion

        #region Protected Fields
        /// <summary>
        /// Reference to the player character.
        /// </summary>
        protected Player m_PlayerCharacter;

        /// <summary>
        /// Reference to the equipment system.
        /// </summary>
        protected Equipper m_Equipper;

        /// <summary>
        /// Cached colors for arrow states.
        /// </summary>
        protected static readonly Color BETTER_COLOR = Color.green;
        protected static readonly Color WORSE_COLOR = Color.red;
        protected static readonly Color SAME_COLOR = Color.white;
        protected static readonly Color INACTIVE_COLOR = Color.grey;
        protected const string UNKNOWN_VALUE = "?";
        #endregion

        #region Unity Methods
        protected virtual void Awake()
        {
            ValidateComponents();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the value and updates the comparison display.
        /// </summary>
        /// <param name="info">The attribute information to compare.</param>
        public override void SetValue(AttributeInfo info)
        {
            if (!ValidateAttributeInfo(info))
            {
                Clear();
                return;
            }

            var item = info.ItemInfo.Item;
            if (!InitializeReferences(info))
            {
                Clear();
                return;
            }

            UpdateComparisonDisplay(item);
        }

        /// <summary>
        /// Clears the comparison display.
        /// </summary>
        public override void Clear()
        {
            if (!Object.ReferenceEquals(m_CurrentValueText, null))
            {
                m_CurrentValueText.text = UNKNOWN_VALUE;
            }
            
            if (m_ArrowImage != null)
            {
                m_ArrowImage.color = INACTIVE_COLOR;
            }
            
            if (!Object.ReferenceEquals(m_NewValueText, null))
            {
                m_NewValueText.text = UNKNOWN_VALUE;
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Validates required components.
        /// </summary>
        protected virtual void ValidateComponents()
        {
            if (string.IsNullOrEmpty(m_StatName))
            {
                Debug.LogWarning($"[CompareAttributeView] Stat name not set on {gameObject.name}", this);
            }

            if (Object.ReferenceEquals(m_CurrentValueText, null))
            {
                Debug.LogError($"[CompareAttributeView] Current value text component missing on {gameObject.name}", this);
            }

            if (Object.ReferenceEquals(m_NewValueText, null))
            {
                Debug.LogError($"[CompareAttributeView] New value text component missing on {gameObject.name}", this);
            }

            if (m_ArrowImage == null)
            {
                Debug.LogError($"[CompareAttributeView] Arrow image component missing on {gameObject.name}", this);
            }
        }

        /// <summary>
        /// Validates the attribute info.
        /// </summary>
        /// <param name="info">The attribute info to validate.</param>
        /// <returns>True if valid, false otherwise.</returns>
        protected virtual bool ValidateAttributeInfo(AttributeInfo info)
        {
            if (info == null) return false;
            return info.Attribute != null;
        }

        /// <summary>
        /// Initializes player and equipper references.
        /// </summary>
        /// <param name="info">The attribute info containing reference data.</param>
        /// <returns>True if initialization successful, false otherwise.</returns>
        protected virtual bool InitializeReferences(AttributeInfo info)
        {
            // Try to get player from inventory first
            if (m_PlayerCharacter == null)
            {
                m_PlayerCharacter = info.ItemInfo.Inventory?.gameObject?.GetComponent<Player>();
            }

            // Fallback to finding in scene
            if (m_PlayerCharacter == null)
            {
                m_PlayerCharacter = FindObjectOfType<Player>();
            }

            // Get or find equipper
            if (m_PlayerCharacter == null)
            {
                m_Equipper = FindObjectOfType<Equipper>();
            }
            else
            {
                m_Equipper = m_PlayerCharacter.GetComponent<Equipper>();
            }

            if (m_Equipper == null)
            {
                Debug.LogWarning("[CompareAttributeView] Player character or its equipper not found for attribute comparison.", gameObject);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Updates the comparison display for the given item.
        /// </summary>
        /// <param name="item">The item to compare.</param>
        protected virtual void UpdateComparisonDisplay(Item item)
        {
            var currentValue = m_Equipper.GetEquipmentStatInt(m_StatName);
            var previewValue = m_Equipper.IsEquipped(item)
                ? m_Equipper.GetEquipmentStatPreviewRemove(m_StatName, item)
                : m_Equipper.GetEquipmentStatPreviewAdd(m_StatName, item);

            m_CurrentValueText.text = currentValue.ToString();
            m_NewValueText.text = previewValue.ToString();
            
            m_ArrowImage.color = GetComparisonColor(currentValue, previewValue);
        }

        /// <summary>
        /// Gets the appropriate color based on value comparison.
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <param name="newValue">The potential new value.</param>
        /// <returns>Color indicating better, worse, or same.</returns>
        protected virtual Color GetComparisonColor(int currentValue, int newValue)
        {
            if (newValue > currentValue) return BETTER_COLOR;
            if (newValue < currentValue) return WORSE_COLOR;
            return SAME_COLOR;
        }
        #endregion
    }
}