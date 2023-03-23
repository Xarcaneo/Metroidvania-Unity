using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newKnockbackData", menuName = "Data/CoreCoponents Data/Knockback Data")]
public class KnockbackData : ScriptableObject
{
    [Header("Knockback parameters")]
    public Vector2 knockbackAngle = new Vector2(1, 0);
    public float knockbackStrength = 2;
    public float maxKnockbackTime = 0.2f;
}
