using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class FlamePlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rayLength = 0.55f;
    
    [Header("Debug Settings")]
    [SerializeField] private bool drawRaycast = true;
    
    [Header("UI References")]
    [SerializeField] private GameObject interactionPromptPrefab;
    [SerializeField] private Vector2 promptOffset = new Vector2(0, 0.5f);

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool canMove = true;
    private GameObject currentPrompt;
    private TextMeshPro promptText;
    private IInteractable currentInteractable;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (interactionPromptPrefab != null)
        {
            currentPrompt = Instantiate(interactionPromptPrefab, transform.position, Quaternion.identity);
            promptText = currentPrompt.GetComponent<TextMeshPro>();
            currentPrompt.SetActive(false);
        }
    }

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

    private void FixedUpdate()
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

    private void HandleMovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

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
            movement = Vector2.zero;
            canMove = true;

            IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
            if (interactable != null && interactable.CanInteract)
            {
                ShowInteractionPrompt(interactable);
                currentInteractable = interactable;
                interactable.Interact();
            }
            else
            {
                HideInteractionPrompt();
                currentInteractable = null;
            }

            return true;
        }
        else
        {
            HideInteractionPrompt();
            currentInteractable = null;
        }

        return false;
    }

    private void ShowInteractionPrompt(IInteractable interactable)
    {
        if (currentPrompt != null && promptText != null)
        {
            promptText.text = interactable.InteractionPrompt;
            currentPrompt.SetActive(true);
        }
    }

    private void HideInteractionPrompt()
    {
        if (currentPrompt != null)
        {
            currentPrompt.SetActive(false);
        }
    }

    private void UpdatePromptPosition()
    {
        if (currentPrompt != null && currentPrompt.activeSelf)
        {
            currentPrompt.transform.position = transform.position + (Vector3)promptOffset;
        }
    }

    private void OnDisable()
    {
        HideInteractionPrompt();
    }
}
