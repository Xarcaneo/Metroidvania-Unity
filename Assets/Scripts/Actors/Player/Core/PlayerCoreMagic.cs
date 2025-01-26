using UnityEngine;
using PixelCrushers.DialogueSystem;

/// <summary>
/// Core component that handles player-specific magic and spell casting functionality.
/// </summary>
public class PlayerCoreMagic : CoreMagic
{
    private const string StatePrefix = "SpellHotbar.";

    /// <summary>
    /// Gets the spell ID from Lua based on the hotbar number.
    /// </summary>
    /// <param name="hotbarNumber">The hotbar number (0-based)</param>
    /// <returns>The spell ID, or 0 if no spell found</returns>
    private int GetSpellIDFromHotbar(int hotbarNumber)
    {
        Lua.Result luaResult = DialogueLua.GetVariable($"{StatePrefix}{hotbarNumber}");
        return luaResult.isNumber ? luaResult.asInt : 0;
    }

    /// <summary>
    /// Gets the cast type of a spell based on the hotbar number.
    /// </summary>
    /// <param name="hotbarNumber">The hotbar number (0-based)</param>
    /// <returns>The CastType of the spell, or CastType.Instant if no spell found</returns>
    public override CastType GetSpellCastType(int hotbarNumber)
    {
        return base.GetSpellCastType(GetSpellIDFromHotbar(hotbarNumber));
    }

    /// <summary>
    /// Gets the channeling time of a spell based on the hotbar number.
    /// </summary>
    /// <param name="hotbarNumber">The hotbar number (0-based)</param>
    /// <returns>The channeling time in seconds, or 0 if no spell found</returns>
    public override float GetSpellChannelingTime(int hotbarNumber)
    {
        return base.GetSpellChannelingTime(GetSpellIDFromHotbar(hotbarNumber));
    }

    /// <summary>
    /// Sets the current spell based on the hotbar number.
    /// </summary>
    /// <param name="hotbarNumber">The hotbar number (0-based)</param>
    /// <returns>True if spell was found and set, false otherwise</returns>
    public override bool SetCurrentSpell(int hotbarNumber)
    {
        return base.SetCurrentSpell(GetSpellIDFromHotbar(hotbarNumber));
    }
}
