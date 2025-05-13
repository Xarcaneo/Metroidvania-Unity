using UnityEngine;

/// <summary>
/// Manages the state of the spell hotbar, tracking which slot is active
/// and handling the differences between keyboard and gamepad input.
/// </summary>
public class HotbarStateManager
{
    private int? activeSlot = null;
    private int lastSlot = 0;

    /// <summary>
    /// Whether any spell slot is currently active
    /// </summary>
    public bool IsSpellActive => activeSlot.HasValue;
    
    /// <summary>
    /// The currently active slot, or the last used slot if none is active
    /// </summary>
    public int CurrentSlot => activeSlot ?? lastSlot;
    
    /// <summary>
    /// For backward compatibility with the old system
    /// </summary>
    public int UseSpellHotbarNumber => CurrentSlot;

    /// <summary>
    /// Activates the specified hotbar slot
    /// </summary>
    public void ActivateSlot(int slot)
    {
        activeSlot = slot;
        Debug.Log($"Hotbar slot activated: {slot}");
    }

    /// <summary>
    /// Deactivates the currently active slot, remembering it as the last used slot
    /// </summary>
    public void DeactivateSlot()
    {
        if (activeSlot.HasValue)
        {
            lastSlot = activeSlot.Value;
            activeSlot = null;
            Debug.Log($"Hotbar slot deactivated, last slot: {lastSlot}");
        }
    }

    /// <summary>
    /// Sets the last used slot without activating it
    /// </summary>
    public void SetLastSlot(int slot)
    {
        lastSlot = slot;
        Debug.Log($"Hotbar last slot set to: {slot}");
    }

    /// <summary>
    /// Resets the hotbar state to default values
    /// </summary>
    public void ResetSlot()
    {
        activeSlot = null;
        lastSlot = 0;
        Debug.Log("Hotbar state reset");
    }
}
