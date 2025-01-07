using UnityEngine;

/// <summary>
/// Data container for enemy melee attack state configuration.
/// Controls melee attack properties and behavior.
/// </summary>
[CreateAssetMenu(fileName = "newMeleeAttackData", menuName = "Data/State Data/Melee Attack State")]
public class D_MeleeAttack : D_BaseState
{
    [Header("Attack Settings")]
    [Tooltip("Amount of damage dealt by the melee attack")]
    [Min(0)]
    public float attackDamage = 10f;

    [Tooltip("Range of the melee attack")]
    [Min(0)]
    public float attackRange = 1.5f;

    [Tooltip("Duration of the attack animation")]
    [Min(0)]
    public float attackDuration = 0.5f;

    private void OnValidate()
    {
        attackDamage = Mathf.Max(0f, attackDamage);
        attackRange = Mathf.Max(0f, attackRange);
        attackDuration = Mathf.Max(0f, attackDuration);
    }
}
