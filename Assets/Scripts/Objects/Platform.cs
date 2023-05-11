using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Platform : MonoBehaviour
{
    [SerializeField] private float timeToRevertCollision = 0.5f;

    private PlatformEffector2D effector;

    private bool playerOnPlatform = false;

    private void OnEnable() => GameEvents.Instance.onPlayerCrouchJump += OnPlayerCrouchJump;
    private void OnDisable() => GameEvents.Instance.onPlayerCrouchJump -= OnPlayerCrouchJump;

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
