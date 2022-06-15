using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRaycast : CoreComponent
{
    private Transform target;
    [SerializeField]
    private Transform raycastOrigin;
    [SerializeField]
    LayerMask collisionLayer;

    private void Start()
    {
       target = Object.FindObjectOfType<Player>().transform;
    }

    public bool CheckRaycastCollision()
    {
        if (target)
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
        else
        {
            return false;
        }
    }
}

