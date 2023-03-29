using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : Interactable
{   
    public Animator Anim { get; private set; }

    public override void Interact()
    {
        Player.Instance.GetComponent<PlayerPositionSaver>().isCheckpoint = true;
        SaveSystem.SaveToSlot(GameManager.Instance.currentSaveSlot);

        Player.Instance.CheckpointInteractionState.SetDetectedPosition(this.transform.position.x);
        Player.Instance.StateMachine.ChangeState(Player.Instance.CheckpointInteractionState);
        Anim.SetBool("activated", true);
    }

    private void Start()
    {
        Anim = GetComponent<Animator>();
    }

    public void AnimationFinished()
    {
        CallInteractionCompletedEvent();
        Anim.SetBool("activated", false);
    }
}
