using UnityEngine;

/// <summary>
/// ScriptableObject that defines knockback parameters for entities.
/// Can be configured in the Unity Inspector and reused across multiple entities.
/// </summary>
[CreateAssetMenu(fileName = "newKnockbackData", menuName = "Data/CoreCoponents Data/Knockback Data")]
public class KnockbackData : ScriptableObject
{
    [Header("Knockback Parameters")]
    [Tooltip("Direction of the knockback force (normalized in-game)")]
    public Vector2 knockbackAngle = new Vector2(1, 0);

    [Tooltip("Strength of the knockback force")]
    public float knockbackStrength = 2;

    [Tooltip("Maximum duration of the knockback effect")]
    public float maxKnockbackTime = 0.2f;
}
