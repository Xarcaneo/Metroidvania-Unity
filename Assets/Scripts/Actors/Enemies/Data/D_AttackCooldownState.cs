using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAttackCooldownData", menuName = "Data/State Data/Attack Cooldown State")]
public class D_AttackCooldownState : ScriptableObject
{
    public float cooldownTime = 2.0f;
}
