/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Integrations.UnityLocalization
{
    using Opsive.Shared.UI;
    using UnityEngine;
    using UnityEngine.Localization.Components;
    using UnityEngine.Localization.Tables;

    public class AutoLocalizeStringEvent : LocalizeStringEvent
    {
        [Tooltip("Target Localize string event")]
        [SerializeField] protected Text m_Text;

        /// <summary>
        /// Automatically get the text component if it is null.
        /// </summary>
        private void Awake()
        {
            if (m_Text.UnityText == null) {
                m_Text.UnityText = GetComponent< UnityEngine.UI.Text>();
            } 
            
#if TEXTMESH_PRO_PRESENT
            if (m_Text.TextMeshProText == null) {
                m_Text.TextMeshProText = GetComponent< TMPro.TMP_Text>();
            }
#endif
        }

        /// <summary>
        /// Set the table entry reference.
        /// </summary>
        /// <param name="tableEntryReference">The table entry reference to set.</param>
        public void SetTableEntryReference(string tableEntryReference)
        {
            StringReference.TableEntryReference = tableEntryReference;
            RefreshString();
        }

        /// <summary>
        /// Set the text to the text component.
        /// </summary>
        /// <param name="value">the new text.</param>
        protected override void UpdateString(string value)
        {
            base.UpdateString(value);
            m_Text.text = value;
        }
    }
}
