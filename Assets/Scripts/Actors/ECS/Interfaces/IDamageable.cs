using UnityEngine;

/// <summary>
/// Interface for entities that can receive damage.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Applies damage to the entity.
    /// </summary>
    /// <param name="damageData">Data containing damage amount, source, and other properties</param>
    void Damage(DamageData damageData);

    /// <summary>
    /// Immediately kills the entity, bypassing normal damage calculations.
    /// </summary>
    void InstantKill();

    /// <summary>
    /// Structure containing damage-related data.
    /// </summary>
    public struct DamageData
    {
        /// <summary>
        /// Amount of damage to apply
        /// </summary>
        public float DamageAmount { get; set; }

        /// <summary>
        /// Entity that caused the damage
        /// </summary>
        public Entity Source { get; set; }

        /// <summary>
        /// Whether this damage can be blocked
        /// </summary>
        public bool CanBlock { get; set; }

        /// <summary>
        /// Sets the basic damage data.
        /// </summary>
        /// <param name="source">Entity causing the damage</param>
        /// <param name="damageAmount">Amount of damage to apply</param>
        public void SetData(Entity source, float damageAmount)
        {
            DamageAmount = damageAmount;
            Source = source;
        }
    }
}
