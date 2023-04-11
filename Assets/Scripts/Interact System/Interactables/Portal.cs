using Com.LuisPedroFonseca.ProCamera2D;
using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Interactable
{
    [SerializeField] ScenePortal m_scenePortal;
    [SerializeField] GameObject m_cameraHook;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FindObjectOfType<ProCamera2D>().AddCameraTarget(m_cameraHook.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FindObjectOfType<ProCamera2D>().RemoveCameraTarget(m_cameraHook.transform);
        }
    }
}
