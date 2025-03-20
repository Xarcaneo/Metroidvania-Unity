using UnityEngine;

/// <summary>
/// Represents the data of a spell, including name, description, icon, and ID.
/// </summary>
[System.Serializable]
public struct SpellData
{
    /// <summary>
    /// The prefix used for spell unlock status in Lua variables.
    /// </summary>
    public const string SPELL_PREFIX = "Spell.";

    /// <summary>
    /// Text to display for locked spell names.
    /// </summary>
    public const string LOCKED_SPELL_NAME = "????";

    /// <summary>
    /// Text to display for locked spell descriptions.
    /// </summary>
    public const string LOCKED_SPELL_DESCRIPTION = "?????????";

    /// <summary>
    /// The name of the spell.
    /// </summary>
    [Tooltip("The name of the spell.")]
    public string SpellName;

    /// <summary>
    /// The description of the spell.
    /// </summary>
    [Tooltip("The description of the spell.")]
    [TextArea(3, 10)]
    public string SpellDescription;

    /// <summary>
    /// The icon representing the spell.
    /// </summary>
    [Tooltip("The icon for the spell.")]
    public Sprite SpellIcon;

    /// <summary>
    /// The unique ID of the spell.
    /// </summary>
    public int SpellID;

    /// <summary>
    /// The unique string ID used to check spell unlock status in Lua variables.
    /// Will be combined with SPELL_PREFIX to form the full variable name.
    /// </summary>
    [Tooltip("The ID used to check if this spell is unlocked.")]
    public string SpellUnlockID;

    /// <summary>
    /// Constructor to initialize the spell data.
    /// </summary>
    /// <param name="spellName">Name of the spell.</param>
    /// <param name="spellDescription">Description of the spell.</param>
    /// <param name="spellIcon">Icon representing the spell.</param>
    /// <param name="spellID">Unique ID of the spell.</param>
    /// <param name="spellUnlockID">Unique string ID for checking spell unlock status.</param>
    public SpellData(string spellName, string spellDescription, Sprite spellIcon, int spellID, string spellUnlockID)
    {
        SpellName = spellName;
        SpellDescription = spellDescription;
        SpellIcon = spellIcon;
        SpellID = spellID;
        SpellUnlockID = spellUnlockID;
    }
}
