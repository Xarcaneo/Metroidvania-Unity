/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Item
{
    using PixelCrushers;
    using PixelCrushers.DialogueSystem;
    using UnityEngine;
    using Text = Opsive.Shared.UI.Text;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Handles the display of item descriptions including name, description text, and rarity icon.
    /// Supports localization through TextTables.
    /// </summary>
    public class ItemDescription : ItemDescriptionBase
    {
        #region Serialized Fields
        [Header("Attribute Settings")]
        [Tooltip("The attribute name for the description")]
        [SerializeField] protected string m_DescriptionAttributeName = "Description";

        [Header("UI Components")]
        [Tooltip("Text component for displaying the item name")]
        [SerializeField] protected Text m_ItemNameText;
        [Tooltip("Text component for displaying the description")]
        [SerializeField] protected Text m_DescriptionText;
        [Tooltip("Icon component for displaying item rarity")]
        [SerializeField] protected RarityIcon m_ItemRarityIcon;

        [Header("Default Messages")]
        [Tooltip("Message shown when no item is selected")]
        [SerializeField] protected string m_NoItemSelectedMessage = "No Item selected";
        [Tooltip("Message shown when item has no description")]
        [SerializeField] protected string m_NoDescriptionMessage = "The item does not have a description";

        [Header("Localization")]
        [Tooltip("Text table for description localization")]
        [SerializeField] private TextTable m_DescriptionTextTable;
        [Tooltip("Text table for item name localization")]
        [SerializeField] private TextTable m_ItemNameTextTable;
        #endregion

        #region Constants
        private const string RARITY_ATTRIBUTE_NAME = "Rarity";
        #endregion

        #region Unity Methods
        protected override void Awake()
        {
            base.Awake();
            ValidateComponents();
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Validates required components and configuration.
        /// </summary>
        protected virtual void ValidateComponents()
        {
            if (string.IsNullOrEmpty(m_DescriptionAttributeName))
            {
                Debug.LogWarning($"[ItemDescription] Description attribute name is empty on {gameObject.name}", this);
            }

            if (Object.ReferenceEquals(m_ItemNameText, null))
            {
                Debug.LogError($"[ItemDescription] Item name text component missing on {gameObject.name}", this);
            }

            if (Object.ReferenceEquals(m_DescriptionText, null))
            {
                Debug.LogError($"[ItemDescription] Description text component missing on {gameObject.name}", this);
            }

            if (m_ItemRarityIcon == null)
            {
                Debug.LogWarning($"[ItemDescription] Item rarity icon component missing on {gameObject.name}", this);
            }

            if (m_DescriptionTextTable == null)
            {
                Debug.LogWarning($"[ItemDescription] Description text table not assigned on {gameObject.name}", this);
            }

            if (m_ItemNameTextTable == null)
            {
                Debug.LogWarning($"[ItemDescription] Item name text table not assigned on {gameObject.name}", this);
            }
        }

        /// <summary>
        /// Updates the UI with the current item's information.
        /// </summary>
        protected override void OnSetValue()
        {
            if (ItemInfo == null || ItemInfo.Item == null)
            {
                OnClear();
                return;
            }

            UpdateItemName();
            UpdateDescription();
            UpdateRarityIcon();
        }

        /// <summary>
        /// Clears all UI elements.
        /// </summary>
        protected override void OnClear()
        {
            if (!Object.ReferenceEquals(m_ItemNameText, null))
            {
                m_ItemNameText.text = m_NoItemSelectedMessage;
            }

            if (m_ItemRarityIcon != null)
            {
                m_ItemRarityIcon.Clear();
            }

            if (!Object.ReferenceEquals(m_DescriptionText, null))
            {
                m_DescriptionText.text = string.Empty;
            }
        }

        /// <summary>
        /// Updates the item name display with localization support.
        /// </summary>
        protected virtual void UpdateItemName()
        {
            if (Object.ReferenceEquals(m_ItemNameText, null) || m_ItemNameTextTable == null) return;

            string localizedName = m_ItemNameTextTable.GetFieldTextForLanguage(
                ItemInfo.Item.name, 
                Localization.Language);

            m_ItemNameText.text = localizedName;
        }

        /// <summary>
        /// Updates the description text with localization support.
        /// </summary>
        protected virtual void UpdateDescription()
        {
            if (Object.ReferenceEquals(m_DescriptionText, null)) return;

            if (!ItemInfo.Item.TryGetAttributeValue<string>(m_DescriptionAttributeName, out var descriptionValue))
            {
                m_DescriptionText.text = m_NoDescriptionMessage;
                return;
            }

            if (m_DescriptionTextTable != null)
            {
                string localizedDescription = m_DescriptionTextTable.GetFieldTextForLanguage(
                    descriptionValue,
                    Localization.Language);
                m_DescriptionText.text = localizedDescription;
            }
            else
            {
                m_DescriptionText.text = descriptionValue;
            }
        }

        /// <summary>
        /// Updates the rarity icon display.
        /// </summary>
        protected virtual void UpdateRarityIcon()
        {
            if (m_ItemRarityIcon == null) return;

            if (ItemInfo.Item.TryGetAttributeValue<ItemRarity>(RARITY_ATTRIBUTE_NAME, out var rarity))
            {
                m_ItemRarityIcon.SetValue(rarity);
            }
            else
            {
                m_ItemRarityIcon.Clear();
            }
        }
        #endregion
    }
}
