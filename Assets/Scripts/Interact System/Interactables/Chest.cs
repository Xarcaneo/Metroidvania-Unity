using Opsive.UltimateInventorySystem.DropsAndPickups;
using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    [SerializeField] int m_ChestID;
    [SerializeField] private RandomItemDropper m_randomItemDropper;

    private Animator chestAnim; // reference to the Animator component

    IEnumerator Start()
    {
        chestAnim = GetComponent<Animator>();

        yield return new WaitForEndOfFrame();
        var chestState = DialogueLua.GetVariable("Chest." + m_ChestID).asBool;
  
        if (chestState) SetOpened();
    }

    public override void Interact()
    {
        base.Interact();

        chestAnim.SetBool("Opening", true);
    }

    private void SetOpened()
    {
        canInteract = false;
        chestAnim.SetBool("IdleOpened", true);
    }

    void OnAnimationTrigger()
    {
        SetOpened();
        DialogueLua.SetVariable("Chest." + m_ChestID, true);

        m_randomItemDropper.Drop();
    }
}
