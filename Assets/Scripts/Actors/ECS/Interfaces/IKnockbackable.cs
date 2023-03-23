using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKnockbackable
{
    void ReceiveKnockback(int direction);
    void ReceiveKnockback();
}