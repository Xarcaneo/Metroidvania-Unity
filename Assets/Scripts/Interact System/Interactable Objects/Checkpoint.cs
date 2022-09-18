using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Interactable
{
    public Animator Anim { get; private set; }

    private void Start()
    {
        Anim = GetComponent<Animator>();
    }

    public override void Interact()
    {
        base.Interact();

        SaveSystem.SaveToSlot(GameManager.Instance.currentSaveSlot);
        Anim.SetBool("activated", true);
    }

    public void AnimationFinished() => Anim.SetBool("activated", false);
}
