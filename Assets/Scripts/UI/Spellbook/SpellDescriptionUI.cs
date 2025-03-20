using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Handles displaying spell descriptions in the UI.
/// </summary>
public class SpellDescriptionUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Text component for displaying the spell name")]
    [SerializeField] private TextMeshProUGUI spellNameText;

    [Tooltip("Text component for displaying the spell description")]
    [SerializeField] private TextMeshProUGUI spellDescriptionText;

    [Tooltip("Image component for displaying the spell icon")]
    [SerializeField] private Image spellIconImage;

    /// <summary>
    /// Updates the UI with information from the given spell.
    /// If the spell is locked, shows placeholder text instead.
    /// </summary>
    /// <param name="spellData">The spell data to display.</param>
    /// <param name="isLocked">Whether the spell is locked.</param>
    public void UpdateSpellDescription(SpellData spellData, bool isLocked)
    {
        if (spellData.Equals(default))
        {
            ClearDescription();
            return;
        }

        // Update spell name
        if (spellNameText != null)
        {
            spellNameText.text = isLocked ? SpellData.LOCKED_SPELL_NAME : spellData.SpellName;
        }

        // Update spell description
        if (spellDescriptionText != null)
        {
            spellDescriptionText.text = isLocked ? SpellData.LOCKED_SPELL_DESCRIPTION : spellData.SpellDescription;
        }

        // Update spell icon
        if (spellIconImage != null)
        {
            spellIconImage.sprite = spellData.SpellIcon;
            spellIconImage.enabled = true;

            // Make the icon semi-transparent if the spell is locked
            Color iconColor = spellIconImage.color;
            iconColor.a = isLocked ? 0.5f : 1f;
            spellIconImage.color = iconColor;
        }
    }

    /// <summary>
    /// Clears all spell information from the UI.
    /// </summary>
    public void ClearDescription()
    {
        if (spellNameText != null)
        {
            spellNameText.text = string.Empty;
        }

        if (spellDescriptionText != null)
        {
            spellDescriptionText.text = string.Empty;
        }

        if (spellIconImage != null)
        {
            spellIconImage.sprite = null;
            spellIconImage.enabled = false;
        }
    }
}
