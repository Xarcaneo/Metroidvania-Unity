using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a category of spells, containing a name, icon, and list of spells.
/// </summary>
[CreateAssetMenu(fileName = "NewSpellCategory", menuName = "SpellBook/SpellCategory")]
public class SpellCategory : ScriptableObject
{
    /// <summary>
    /// The name of the spell category.
    /// </summary>
    [Tooltip("The name of the spell category.")]
    public string categoryName;

    /// <summary>
    /// The icon representing the spell category.
    /// </summary>
    [Tooltip("The icon representing the spell category.")]
    public Sprite categoryIcon;

    /// <summary>
    /// List of spells belonging to this category.
    /// </summary>
    [Tooltip("List of spells in this category.")]
    public List<Spell> spells = new List<Spell>();
}
