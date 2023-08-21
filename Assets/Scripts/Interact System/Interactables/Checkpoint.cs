using PixelCrushers;
using UnityEngine;

public class Checkpoint : Interactable
{
    [SerializeField] float _playerXOffset = -0.5f;

    public Animator Anim { get; private set; }

    public override void Interact()
    {
        GameEvents.Instance.GameSaving();

        Player.Instance.gameObject.transform.position = 
            new Vector3(this.transform.position.x + _playerXOffset, Player.Instance.gameObject.transform.position.y, 0);
        Player.Instance.gameObject.SetActive(false);

        Anim.SetBool("activated", true);
    }

    private void Start()
    {
        Anim = GetComponent<Animator>();
    }

    public void AnimationFinished()
    {
        if (Player.Instance.Core.GetCoreComponent<Movement>().FacingDirection != this.transform.localScale.x)
            Player.Instance.Core.GetCoreComponent<Movement>().Flip();

        Player.Instance.gameObject.SetActive(true);

        Player.Instance.GetComponent<PlayerPositionSaver>().isCheckpoint = true;
        SaveSystem.SaveToSlot(GameManager.Instance.currentSaveSlot);

        CallInteractionCompletedEvent();
        Anim.SetBool("activated", false);
    }
}
