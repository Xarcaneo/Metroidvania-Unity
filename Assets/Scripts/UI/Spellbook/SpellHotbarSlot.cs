using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a single slot in the SpellHotbar.
/// Manages the assigned spell and handles related UI updates.
/// </summary>
public class SpellHotbarSlot : MonoBehaviour
{
    [Header("Slot Settings")]
    [Tooltip("The spell currently assigned to this slot.")]
    private Spell assignedSpell;

    [Header("UI Components")]
    [Tooltip("The Image component for displaying the spell's icon.")]
    [SerializeField] private Image spellIcon;

    /// <summary>
    /// Assigns a spell to this slot.
    /// Updates the icon if a spell is assigned or clears the icon if null.
    /// </summary>
    /// <param name="spell">The spell to assign.</param>
    public void AssignSpell(Spell spell)
    {
        assignedSpell = spell;

        if (assignedSpell != null)
        {
            // Update the spell icon
            if (spellIcon != null)
            {
                spellIcon.sprite = assignedSpell.spellData.SpellIcon; // Assuming SpellData has a "SpellIcon" property
                spellIcon.enabled = true; // Make sure the icon is visible
            }
        }
        else
        {
            Debug.LogWarning("AssignedSpell is null. Clearing the slot.");

            // Clear the spell icon
            if (spellIcon != null)
            {
                spellIcon.sprite = null;
                spellIcon.enabled = false; // Optionally hide the icon
            }
        }
    }

    /// <summary>
    /// Gets the spell currently assigned to this slot.
    /// </summary>
    public Spell GetAssignedSpell()
    {
        return assignedSpell;
    }
}
