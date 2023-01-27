using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Interactable
{   
    public Animator Anim { get; private set; }

    public override void Interact()
    {
        isInteractionCompleted = false;

        Player.Instance.GetComponent<PlayerPositionSaver>().m_lastCheckpointPosition = this.transform.position;
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
        isInteractionCompleted = true;
        CallInteractionCompletedEvent();
        Anim.SetBool("activated", false);
    }
}
