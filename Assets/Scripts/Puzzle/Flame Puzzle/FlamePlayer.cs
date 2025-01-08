using UnityEngine;
using TMPro;

/// <summary>
/// Controls the player character in the flame puzzle, handling movement and interactions.
/// Requires a Rigidbody2D component for physics-based movement.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class FlamePlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Movement speed of the player")]
    [SerializeField] private float speed = 5f;
    
    [Tooltip("Length of the raycast used for collision detection")]
    [SerializeField] private float rayLength = 0.55f;
    
    [Header("Debug Settings")]
    [Tooltip("Whether to draw debug raycasts in the Scene view")]
    [SerializeField] private bool drawRaycast = true;
    
    [Header("UI References")]
    [Tooltip("Prefab for the interaction prompt UI")]
    [SerializeField] private GameObject interactionPromptPrefab;
    
    [Tooltip("Offset position for the interaction prompt relative to the player")]
    [SerializeField] private Vector2 promptOffset = new Vector2(0, 0.5f);

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool canMove = true;
    private GameObject currentPrompt;
    private TextMeshPro promptText;
    private IInteractable currentInteractable;

    /// <summary>
    /// Initializes the player's components and UI elements.
    /// </summary>
    private void Start()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Handles input processing and updates UI elements every frame.
    /// </summary>
    private void Update()
    {
        if (canMove)
        {
            HandleMovementInput();
            if (movement != Vector2.zero)
                canMove = false;
        }

        UpdatePromptPosition();
    }

    /// <summary>
    /// Handles physics-based movement in fixed time intervals.
    /// </summary>
    private void FixedUpdate()
    {
        HandleMovement();
    }

    /// <summary>
    /// Initializes required components and UI elements.
    /// </summary>
    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        if (interactionPromptPrefab != null)
        {
            currentPrompt = Instantiate(interactionPromptPrefab, transform.position, Quaternion.identity);
            promptText = currentPrompt.GetComponent<TextMeshPro>();
            currentPrompt.SetActive(false);
        }
    }

    /// <summary>
    /// Processes player input for movement and determines movement direction.
    /// </summary>
    private void HandleMovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Determine movement direction based on dominant input axis
        if (Mathf.Abs(horizontalInput) > Mathf.Abs(verticalInput))
        {
            movement = new Vector2(horizontalInput, 0).normalized;
        }
        else if (Mathf.Abs(verticalInput) > Mathf.Abs(horizontalInput))
        {
            movement = new Vector2(0, verticalInput).normalized;
        }
        else
        {
            movement = Vector2.zero;
        }

        if (CastMovementRay())
        {
            movement = Vector2.zero;
        }
    }

    /// <summary>
    /// Applies movement to the Rigidbody2D and checks for collisions.
    /// </summary>
    private void HandleMovement()
    {
        if (movement != Vector2.zero)
        {
            rb.velocity = movement * speed;
            CastMovementRay();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Casts a ray in the movement direction to check for collisions and interactable objects.
    /// </summary>
    /// <returns>True if the ray hits something, false otherwise.</returns>
    private bool CastMovementRay()
    {
        int playerLayer = 1 << 13;
        int layerMask = ~playerLayer;

        if (drawRaycast)
        {
            Debug.DrawRay(transform.position, movement * rayLength, Color.green);
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, movement, rayLength, layerMask);
        if (hit.collider != null)
        {
            HandleCollisionWithInteractable(hit);
            return true;
        }

        if (currentInteractable != null)
        {
            HideInteractionPrompt();
            currentInteractable = null;
        }

        return false;
    }

    /// <summary>
    /// Processes collision with an interactable object, updating UI and interaction state.
    /// </summary>
    /// <param name="hit">Information about the raycast hit</param>
    private void HandleCollisionWithInteractable(RaycastHit2D hit)
    {
        movement = Vector2.zero;
        canMove = true;

        var interactable = hit.collider.GetComponent<IInteractable>();
        if (interactable != null && interactable.CanInteract)
        {
            interactable.Interact();
        }
    }

    /// <summary>
    /// Displays the interaction prompt with the specified text.
    /// </summary>
    /// <param name="text">Text to display in the prompt</param>
    private void ShowInteractionPrompt(string text)
    {
        if (promptText != null)
        {
            promptText.text = text;
            currentPrompt.SetActive(true);
        }
    }

    /// <summary>
    /// Hides the interaction prompt UI.
    /// </summary>
    private void HideInteractionPrompt()
    {
        if (currentPrompt != null)
        {
            currentPrompt.SetActive(false);
        }
    }

    /// <summary>
    /// Updates the position of the interaction prompt to follow the player.
    /// </summary>
    private void UpdatePromptPosition()
    {
        if (currentPrompt != null && currentPrompt.activeSelf)
        {
            currentPrompt.transform.position = transform.position + (Vector3)promptOffset;
        }
    }

    /// <summary>
    /// Hides the interaction prompt when the player is disabled.
    /// </summary>
    private void OnDisable()
    {
        HideInteractionPrompt();
    }
}
