using UnityEngine;

/// <summary>
/// Data container for enemy attack cooldown state configuration.
/// Controls how long the enemy must wait between attacks.
/// </summary>
[CreateAssetMenu(fileName = "newAttackCooldownData", menuName = "Data/State Data/Attack Cooldown State")]
public class D_AttackCooldownState : D_BaseState
{
    [Header("Cooldown Settings")]
    [Tooltip("Time to wait before allowing another attack")]
    [Min(0)]
    public float cooldownTime = 2.0f;

    private void OnValidate()
    {
        cooldownTime = Mathf.Max(0f, cooldownTime);
    }
}
