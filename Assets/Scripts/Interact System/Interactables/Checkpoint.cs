using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : Interactable
{
    [SerializeField] float _playerXOffset = -0.5f;

    public Animator Anim { get; private set; }

    public override void Interact()
    {
        Player.Instance.GetComponent<PlayerPositionSaver>().isCheckpoint = true;
        SaveSystem.SaveToSlot(GameManager.Instance.currentSaveSlot);

        GameEvents.Instance.DeactivatePlayerInput(true);
        Player.Instance.gameObject.transform.position = 
            new Vector3(this.transform.position.x + _playerXOffset, Player.Instance.gameObject.transform.position.y, 0);
        Player.Instance.gameObject.GetComponent<Renderer>().enabled = false;

        Anim.SetBool("activated", true);
    }

    private void Start()
    {
        Anim = GetComponent<Animator>();
    }

    public void AnimationFinished()
    {
        GameEvents.Instance.DeactivatePlayerInput(false);

        if (Player.Instance.Core.GetCoreComponent<Movement>().FacingDirection != this.transform.localScale.x)
            Player.Instance.Core.GetCoreComponent<Movement>().Flip();

        Player.Instance.gameObject.GetComponent<Renderer>().enabled = true;

        CallInteractionCompletedEvent();
        Anim.SetBool("activated", false);
    }
}
