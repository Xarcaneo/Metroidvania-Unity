using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEssence : Interactable
{
    public override void Interact()
    {
        Destroy(gameObject);
    }
}
