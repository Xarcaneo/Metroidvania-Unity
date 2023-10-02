using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKnockbackable
{
    void ReceiveKnockback(IDamageable.DamageData damageData, int direction);
    void ReceiveKnockback();
}