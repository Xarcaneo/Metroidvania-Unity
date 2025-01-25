using UnityEngine;
using TMPro;

/// <summary>
/// Manages the UI for displaying the name and description of a spell.
/// </summary>
public class SpellDescriptionUI : MonoBehaviour
{
    [Header("UI Elements")]
    /// <summary>
    /// The TextMeshProUGUI element for displaying the spell's name.
    /// </summary>
    [Tooltip("UI element for the spell's name.")]
    [SerializeField] private TextMeshProUGUI spellNameText;

    /// <summary>
    /// The TextMeshProUGUI element for displaying the spell's description.
    /// </summary>
    [Tooltip("UI element for the spell's description.")]
    [SerializeField] private TextMeshProUGUI spellDescriptionText;

    /// <summary>
    /// Updates the UI with the provided spell name and description.
    /// </summary>
    /// <param name="spellName">The name of the spell.</param>
    /// <param name="spellDescription">The description of the spell.</param>
    public void UpdateSpellDescription(string spellName, string spellDescription)
    {
        if (spellNameText != null)
        {
            spellNameText.text = spellName;
        }
        else
        {
            Debug.LogWarning("Spell name text is not assigned.");
        }

        if (spellDescriptionText != null)
        {
            spellDescriptionText.text = spellDescription;
        }
        else
        {
            Debug.LogWarning("Spell description text is not assigned.");
        }
    }

    /// <summary>
    /// Clears the spell description UI.
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
    }
}
