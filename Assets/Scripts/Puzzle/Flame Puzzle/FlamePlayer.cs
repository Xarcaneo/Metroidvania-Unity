using UnityEngine;

public class FlamePlayer : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private bool drawRaycast = true; // Toggle this in the Inspector to draw raycast for debugging
    [SerializeField] private float rayLength = 0.55f; // Adjustable raycast length

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool canMove = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (canMove)
        {
            HandleMovementInput();

            if (movement != Vector2.zero)
                canMove = false;
        }
    }

    private void FixedUpdate()
    {
        // Apply movement if there's input
        if (movement != Vector2.zero)
        {
            rb.velocity = movement * speed;
            CastMovementRay(); // Constantly cast a ray in the direction of movement
        }
        else
        {
            // Optional: Stop the Rigidbody if there's no input
            rb.velocity = Vector2.zero;
        }
    }

    private void HandleMovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Determine which direction has the greater input magnitude
        if (Mathf.Abs(horizontalInput) > Mathf.Abs(verticalInput))
        {
            // Horizontal movement
            movement = new Vector2(horizontalInput, 0).normalized;
        }
        else if (Mathf.Abs(verticalInput) > Mathf.Abs(horizontalInput))
        {
            // Vertical movement
            movement = new Vector2(0, verticalInput).normalized;
        }
        else
        {
            // This case handles when both inputs are equal; choose to not move or prioritize one direction
            movement = Vector2.zero; // Example: Not moving if inputs are equal
        }

        // Perform a raycast in the intended direction; if something is hit, stop movement
        if (CastMovementRay())
        {
            movement = Vector2.zero;
        }
    }


    // Casts and draws a ray in the direction the player is moving
    private bool CastMovementRay()
    {
        int playerLayer = 1 << 13; // Layer 8 for the player
        int layerMask = ~playerLayer; // Invert to get a mask that ignores the player layer

        if (drawRaycast)
        {
            Debug.DrawRay(transform.position, movement * rayLength, Color.green);
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, movement, rayLength, layerMask);
        if (hit.collider != null)
        {
            movement = Vector2.zero;
            canMove = true;

            // Check if the hit object implements IInteractable
            IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
            if (interactable != null)
            {
                // If it does, call the Interact method
                interactable.Interact();
            }

            return true;
        }

        return false;
    }
}
