using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Interactable
{
    public override void Interact()
    {
        base.Interact();

        SaveSystem.SaveToSlot(GameManager.Instance.currentSaveSlot);
        Debug.Log("Checkpoint Activated");
    }
}
