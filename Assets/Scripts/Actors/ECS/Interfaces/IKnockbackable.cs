using UnityEngine;

/// <summary>
/// Interface for entities that can be knocked back.
/// </summary>
public interface IKnockbackable
{
    /// <summary>
    /// Applies knockback to the entity based on damage data and direction.
    /// </summary>
    /// <param name="damageData">Data about the damage causing the knockback</param>
    /// <param name="direction">Direction of the knockback (-1 for left, 1 for right)</param>
    void ReceiveKnockback(IDamageable.DamageData damageData, int direction);

    /// <summary>
    /// Applies knockback to the entity in the opposite direction of its facing direction.
    /// </summary>
    void ReceiveKnockback();
}