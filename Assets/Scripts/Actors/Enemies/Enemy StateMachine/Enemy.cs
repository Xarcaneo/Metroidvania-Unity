using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public D_Entity entityData;

    public override State GetDeathState()
    {
        return null;
    }

    public override State GetHurtState()
    {
        return null;
    }
}
