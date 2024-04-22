namespace Opsive.UltimateInventorySystem.Integrations.UnityLocalization
{
    using System;
    using Opsive.Shared.UI;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
    using Opsive.UltimateInventorySystem.UI.Views;
    using UnityEngine;
    using UnityEngine.Localization.Components;

    public class LocalizeItemView : ItemViewModule
    {
        [Tooltip("The text component that will be used to display the text.")]
        [SerializeField] protected Text m_Text;
        [Tooltip("Target Localize string event")]
        [SerializeField] protected LocalizeStringEvent m_LocalizeStringEvent;
        [Tooltip("The attribute name. Must be an integer.")]
        [SerializeField] protected string m_AttributeName;
        [Tooltip("Default text value.")]
        [SerializeField] protected string m_DefaultTextValue;
        [Tooltip("Disable If the Item or Attribute is Missing.")]
        [SerializeField] protected bool m_ClearOnAttributeMissing;
        [Tooltip("Disable If the Item or Attribute is Missing.")]
        [SerializeField] protected GameObject m_DisableOnClear;
        [Header("Formatting values:", order = 0)]
        [Header("{0} Item Name", order = 1)]
        [Space(-10, order = 2)]
        [Header("{1} Attribute Value", order = 3)]
        [Space(-10, order = 4)]
        [Header("{2} Item Definition Name", order = 5)]
        [Space(-10, order = 6)]
        [Header("{3} Item Category Name", order = 7)]
        [Space(-10, order = 8)]
        [Header("{4} Attribute Name", order = 9)]
        [Tooltip("If the Key Format is empty the item name is used as key.")]
        [SerializeField] protected string m_KeyFormat;
        [Tooltip("Should the formatted key have it's white spaces replaced by underscores?")]
        [SerializeField] protected bool m_ReplaceWhiteSpaceWithUnderscores;

        public Text Text
        {
            get => m_Text;
            set => m_Text = value;
        }

        public bool ReplaceWhiteSpaceWithUnderscores
        {
            get => m_ReplaceWhiteSpaceWithUnderscores;
            set => m_ReplaceWhiteSpaceWithUnderscores = value;
        }

        public LocalizeStringEvent LocalizeStringEvent
        {
            get => m_LocalizeStringEvent;
            set => m_LocalizeStringEvent = value;
        }

        public string DefaultTextValue
        {
            get => m_DefaultTextValue;
            set => m_DefaultTextValue = value;
        }

        public bool ClearOnAttributeMissing
        {
            get => m_ClearOnAttributeMissing;
            set => m_ClearOnAttributeMissing = value;
        }

        public GameObject DisableOnClear
        {
            get => m_DisableOnClear;
            set => m_DisableOnClear = value;
        }

        public string KeyFormat
        {
            get => m_KeyFormat;
            set => m_KeyFormat = value;
        }

        public string AttributeName {
            get => m_AttributeName;
            set => m_AttributeName = value;
        }

        /// <summary>
        /// Initialize the Item View.
        /// </summary>
        /// <param name="view">the parent view.</param>
        public override void Initialize(View view)
        {
            if (m_LocalizeStringEvent == null) {
                m_LocalizeStringEvent = GetComponent<LocalizeStringEvent>();
            }
            base.Initialize(view);
        }

        /// <summary>
        /// Set the value.
        /// </summary>
        /// <param name="info">The item info.</param>
        public override void SetValue(ItemInfo info)
        {
            if (info.Item == null || info.Item.IsInitialized == false) {
                Clear();
                return;
            }
            
            var attribute = info.Item.GetAttribute(m_AttributeName);
            if (m_ClearOnAttributeMissing && attribute == null) {
                Clear();
                return;
            }
            
            string referenceKey;
            if (string.IsNullOrWhiteSpace(m_KeyFormat) == false) {
                referenceKey = string.Format(m_KeyFormat, 
                    info.Item.name,
                    attribute?.GetValueAsObject()?.ToString() ?? "",
                    info.Item.ItemDefinition.name,
                    info.Item.Category.name,
                    m_AttributeName
                    );
            } else {
                referenceKey = info.Item.name;
            }

            if (m_ReplaceWhiteSpaceWithUnderscores) {
                referenceKey = referenceKey.Replace(" ", "_");
            }

            if (m_DisableOnClear != null) {
                m_DisableOnClear.SetActive(true);
            }

            m_LocalizeStringEvent.StringReference.TableEntryReference = referenceKey;
            if (string.IsNullOrWhiteSpace(referenceKey)|| m_LocalizeStringEvent == null || m_LocalizeStringEvent.StringReference == null || m_LocalizeStringEvent.StringReference.IsEmpty) {
                m_Text.SetText(referenceKey);
            } else {
                m_Text.SetText(m_LocalizeStringEvent.StringReference.GetLocalizedString());
            }
        }

        /// <summary>
        /// Clear the value.
        /// </summary>
        public override void Clear()
        {
            m_LocalizeStringEvent.StringReference.TableEntryReference = m_DefaultTextValue;
            if (string.IsNullOrWhiteSpace(m_DefaultTextValue) || m_LocalizeStringEvent == null || m_LocalizeStringEvent.StringReference == null || m_LocalizeStringEvent.StringReference.IsEmpty) {
                m_Text.SetText(m_DefaultTextValue);
            } else {
                m_Text.SetText(m_LocalizeStringEvent.StringReference.GetLocalizedString());
            }

            if (m_DisableOnClear != null) {
                m_DisableOnClear.SetActive(false);
            }
        }
    }
}