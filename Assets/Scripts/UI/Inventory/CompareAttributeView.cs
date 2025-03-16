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

        [Tooltip("Background GameObject for the comparison value")]
        [SerializeField] protected GameObject m_ComparisonBackground;

        [Tooltip("Color for when the comparison value shows an improvement")]
        [SerializeField] protected Color m_BetterColor = Color.green;

        [Tooltip("Color for when the comparison value shows a decrease")]
        [SerializeField] protected Color m_WorseColor = Color.red;

        [Tooltip("Color for when the comparison value shows no change")]
        [SerializeField] protected Color m_SameColor = Color.white;

        [Tooltip("Color for inactive or unknown state")]
        [SerializeField] protected Color m_InactiveColor = Color.grey;

        /// <summary>
        /// Constant for unknown value display
        /// </summary>
        protected const string UNKNOWN_VALUE = "?";
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
            
            if (!Object.ReferenceEquals(m_NewValueText, null))
            {
                m_NewValueText.text = UNKNOWN_VALUE;
                m_NewValueText.color = m_InactiveColor;
            }
            m_ComparisonBackground.SetActive(false);
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

            if (m_ComparisonBackground == null)
            {
                Debug.LogError($"[CompareAttributeView] Comparison background GameObject missing on {gameObject.name}", this);
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
            // Get the item's actual stat value
            var itemStatValue = 0;
            if (item.TryGetAttribute(m_StatName, out var attribute))
            {
                if (attribute.GetValueAsObject() is int value)
                {
                    itemStatValue = value;
                }
            }

            // Display item's actual stat value
            m_CurrentValueText.text = itemStatValue.ToString();

            // If item is equipped, hide the comparison background
            if (m_Equipper.IsEquipped(item))
            {
                m_NewValueText.gameObject.SetActive(false);
            }
            else
            {
                m_NewValueText.gameObject.SetActive(true);
            }

            // Make sure the comparison background is visible for unequipped items
            m_ComparisonBackground.SetActive(true);

            // Calculate how this item will change stats if equipped
            var currentEquippedValue = m_Equipper.GetEquipmentStatInt(m_StatName);
            var previewValue = m_Equipper.GetEquipmentStatPreviewAdd(m_StatName, item);
            int difference = previewValue - currentEquippedValue;

            // Display the difference with + or - sign and appropriate color
            m_NewValueText.text = difference > 0 ? $"+{difference}" : difference.ToString();
            
            // Set color based on the difference
            if (difference > 0)
            {
                m_NewValueText.color = m_BetterColor;
            }
            else if (difference < 0)
            {
                m_NewValueText.color = m_WorseColor;
            }
            else
            {
                m_NewValueText.color = m_SameColor;
            }
        }
        #endregion
    }
}