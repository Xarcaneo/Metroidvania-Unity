using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamePlayer : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    private bool canMove = true;
    private Vector2 movement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (canMove)
        {
            HandleMovementInput();
        }

        rb.velocity = movement;
    }

    private void HandleMovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        movement = new Vector2(horizontalInput, verticalInput).normalized * speed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Player can move when any collision occurs
        canMove = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        canMove = false;
    }
}
