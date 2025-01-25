using UnityEngine;

/// <summary>
/// Represents the data of a spell, including name, description, icon, and ID.
/// </summary>
[System.Serializable]
public struct SpellData
{
    /// <summary>
    /// The name of the spell.
    /// </summary>
    public string SpellName;

    /// <summary>
    /// The description of the spell.
    /// </summary>
    public string SpellDescription;

    /// <summary>
    /// The icon representing the spell.
    /// </summary>
    public Sprite SpellIcon;

    /// <summary>
    /// The unique ID of the spell.
    /// </summary>
    public int SpellID;

    /// <summary>
    /// Constructor to initialize the spell data.
    /// </summary>
    /// <param name="spellName">Name of the spell.</param>
    /// <param name="spellDescription">Description of the spell.</param>
    /// <param name="spellIcon">Icon representing the spell.</param>
    /// <param name="spellID">Unique ID of the spell.</param>
    public SpellData(string spellName, string spellDescription, Sprite spellIcon, int spellID)
    {
        SpellName = spellName;
        SpellDescription = spellDescription;
        SpellIcon = spellIcon;
        SpellID = spellID;
    }
}
