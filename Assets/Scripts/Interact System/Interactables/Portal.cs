using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Interactable
{
    [SerializeField] ScenePortal m_scenePortal;

    public Animator Anim { get; private set; }
    private void Start()
    {
        Anim = GetComponent<Animator>();
    }

    public override void Interact()
    {
        GameEvents.Instance.DeactivatePlayerInput(true);
        Anim.SetBool("activated", true);
    }

    public void AnimationFinished()
    {
        m_scenePortal.UsePortal();
    }

    public void AnimationTriggerEvent()
    {
        Player.Instance.gameObject.SetActive(false);
    }
}
