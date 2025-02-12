/// <summary>
/// Defines how a spell is cast, affecting its casting behavior and animations.
/// </summary>
public enum CastType
{
    /// <summary>
    /// Spell is cast immediately with no charging or channeling.
    /// </summary>
    Instant,
    
    /// <summary>
    /// Spell requires holding the cast button to channel its effects.
    /// </summary>
    Channeled,
    
    /// <summary>
    /// Spell can be charged up before release for increased effect.
    /// </summary>
    Charged,
}
