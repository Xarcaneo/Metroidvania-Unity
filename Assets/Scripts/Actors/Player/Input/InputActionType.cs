using System;

/// <summary>
/// Enum defining all possible input action types
/// </summary>
public enum InputActionType
{
    Attack,
    Block,
    Move,
    [Obsolete("UseSpell is deprecated. Use UseSpellSlot0, UseSpellSlot1, or UseSpellSlot2 instead.")]
    UseSpell,       // Legacy action, will be replaced by individual slot actions
    UseSpellSlot0,  // First spell slot (index 0)
    UseSpellSlot1,  // Second spell slot (index 1)
    UseSpellSlot2,  // Third spell slot (index 2)
    Jump,
    Dash,
    Interact,
    PlayerMenu,
    HotbarAction,
    ItemSwitchLeft,
    ItemSwitchRight
}
