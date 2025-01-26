using UnityEngine;

/// <summary>
/// Core component that handles magic and spell casting functionality.
/// </summary>
public class CoreMagic : CoreComponent
{
    #region Variables

    [Header("UI References")]
    [SerializeField, Tooltip("Reference to the channeling bar sprite")]
    protected GameObject channelingBar;

    /// <summary>
    /// The currently active spell
    /// </summary>
    protected internal Spell currentSpell;

    /// <summary>
    /// ID of the current LeanTween animation
    /// </summary>
    protected int currentTweenId = -1;

    #endregion

    #region Unity Callback Methods

    protected override void Awake()
    {
        base.Awake();
        if (channelingBar != null)
        {
            channelingBar.SetActive(false);
        }
    }

    protected virtual void OnDisable()
    {
        StopChannelingBar();
    }

    #endregion

    #region Main Methods

    /// <summary>
    /// Starts animating the channeling bar, scaling it from 1 to 0 over the channeling duration.
    /// </summary>
    /// <param name="channelingTime">Duration of the channeling in seconds</param>
    public virtual void StartChannelingBar(float channelingTime)
    {
        if (channelingBar == null) return;

        // Stop any existing animation
        StopChannelingBar();

        // Reset and show the bar
        channelingBar.transform.localScale = Vector3.one;
        channelingBar.SetActive(true);

        // Animate the bar's scale from 1 to 0 on X axis
        currentTweenId = LeanTween.scaleX(channelingBar, 0f, channelingTime)
            .setEaseLinear()
            .uniqueId;
    }

    /// <summary>
    /// Stops the channeling bar animation and hides the bar.
    /// </summary>
    public virtual void StopChannelingBar()
    {
        if (channelingBar == null) return;

        // Cancel existing tween if any
        if (currentTweenId != -1)
        {
            LeanTween.cancel(currentTweenId);
            currentTweenId = -1;
        }

        // Hide and reset the bar
        channelingBar.SetActive(false);
        channelingBar.transform.localScale = Vector3.one;
    }

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
