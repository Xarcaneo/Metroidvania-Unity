using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Platform : MonoBehaviour
{
    [SerializeField] private float timeToRevertCollision = 0.5f;

    private PlayerInputHandler m_playerInputHandler;

    private PlatformEffector2D effector;

    private bool playerOnPlatform = false;

    private void Start() => effector = GetComponent<PlatformEffector2D>();
    

    private void OnPlayerCrouchJump()
    {
        if (playerOnPlatform)
        {
            effector.colliderMask &= ~(1 << LayerMask.NameToLayer("Player"));
            Player.Instance.JumpState.canJump = false;
            StartCoroutine(RevertCollision());
        }
    }

    private void Update()
    {
        if (m_playerInputHandler == null)
        {
            m_playerInputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputHandler>();
        }
        else
        {
            var yInput = m_playerInputHandler.NormInputY;
            var jumpInput = m_playerInputHandler.JumpInput;

            if(jumpInput && yInput == -1)
                OnPlayerCrouchJump();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        int playerLayer = LayerMask.NameToLayer("Player");

        if (collision.gameObject.layer == playerLayer)
        {
            playerOnPlatform = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int playerLayer = LayerMask.NameToLayer("Player");

        if (collision.gameObject.layer == playerLayer)
        {
            playerOnPlatform = true;
        }
    }

    private IEnumerator RevertCollision()
    {
        yield return new WaitForSeconds(timeToRevertCollision);

        Player.Instance.JumpState.canJump = true;
        effector.colliderMask |= (1 << LayerMask.NameToLayer("Player"));
    }
}
