using UnityEngine;

/// <summary>
/// Core component that handles magic and spell casting functionality.
/// </summary>
public class CoreMagic : CoreComponent
{
    #region Variables

    /// <summary>
    /// The currently active spell
    /// </summary>
    protected internal Spell currentSpell;
    #endregion

    #region Unity Callback Methods

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region Main Methods

    /// <summary>
    /// Sets the current spell by its ID from the SpellsCatalogue.
    /// </summary>
    /// <param name="spellID">The ID of the spell to set</param>
    /// <returns>True if spell was found and set, false otherwise</returns>
    public virtual bool SetCurrentSpell(int spellID)
    {
        currentSpell = SpellsCatalogue.Instance.GetSpellByID(spellID);
        return currentSpell != null;
    }

    /// <summary>
    /// Gets the cast type of a spell by its ID from the SpellsCatalogue.
    /// </summary>
    /// <param name="spellID">The ID of the spell</param>
    /// <returns>The CastType of the spell, or CastType.Instant if spell not found</returns>
    public virtual CastType GetSpellCastType(int spellID)
    {
        Spell spell = SpellsCatalogue.Instance.GetSpellByID(spellID);
        return spell != null ? spell.castType : CastType.Instant;
    }

    /// <summary>
    /// Gets the channeling time of a spell by its ID from the SpellsCatalogue.
    /// </summary>
    /// <param name="spellID">The ID of the spell</param>
    /// <returns>The channeling time in seconds, or 0 if spell not found</returns>
    public virtual float GetSpellChannelingTime(int spellID)
    {
        Spell spell = SpellsCatalogue.Instance.GetSpellByID(spellID);
        return spell != null ? spell.channelingTime : 0f;
    }

    #endregion
}
