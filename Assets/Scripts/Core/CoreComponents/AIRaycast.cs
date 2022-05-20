using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRaycast : CoreComponent
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Transform raycastOrigin;
    [SerializeField]
    LayerMask collisionLayer;

    public bool CheckRaycastCollision()
    {
        RaycastHit2D hit = Physics2D.Linecast(raycastOrigin.position, target.position, collisionLayer);

        if (hit.collider)
        {
           return false;
        }
        else
        {
            return true;
        }
    }
}

