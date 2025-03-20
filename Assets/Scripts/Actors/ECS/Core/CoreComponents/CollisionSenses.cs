using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all collision detection and environmental sensing for an entity.
/// Provides methods to check for ground, walls, ledges, ladders, and platforms.
/// </summary>
public class CollisionSenses : CoreComponent
{
    #region Dependencies

    /// <summary>
    /// Reference to the Movement component for directional checks.
    /// </summary>
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    #endregion

    #region Slope Properties

    /// <summary>
    /// Current downward angle of the slope the entity is on.
    /// </summary>
    public float slopeDownAngle;

    /// <summary>
    /// Perpendicular normal vector of the current slope.
    /// Used for movement calculations on slopes.
    /// </summary>
    public Vector2 slopeNormalPerp;

    #endregion

    #region Check Points

    /// <summary>
    /// Transform marking the point for ground detection.
    /// </summary>
    [SerializeField, Tooltip("Point from which ground checks originate")]
    private Transform groundCheck;
    public Transform GroundCheck => GenericNotImplementedError<Transform>.TryGet(groundCheck, core.transform.parent.name);

    /// <summary>
    /// Transform marking the point for wall detection.
    /// </summary>
    [SerializeField, Tooltip("Point from which wall checks originate")]
    private Transform wallCheck;
    public Transform WallCheck => GenericNotImplementedError<Transform>.TryGet(wallCheck, core.transform.parent.name);

    /// <summary>
    /// Transform marking the point for horizontal ledge detection.
    /// </summary>
    [SerializeField, Tooltip("Point from which horizontal ledge checks originate")]
    private Transform ledgeCheckHorizontal;
    public Transform LedgeCheckHorizontal => GenericNotImplementedError<Transform>.TryGet(ledgeCheckHorizontal, core.transform.parent.name);

    /// <summary>
    /// Transform marking the point for vertical ledge detection.
    /// </summary>
    [SerializeField, Tooltip("Point from which vertical ledge checks originate")]
    private Transform ledgeCheckVertical;
    public Transform LedgeCheckVertical => GenericNotImplementedError<Transform>.TryGet(ledgeCheckVertical, core.transform.parent.name);

    /// <summary>
    /// Transform marking the point for ladder detection.
    /// </summary>
    [SerializeField, Tooltip("Point from which ladder checks originate")]
    private Transform ladderCheck;
    public Transform LadderCheck => GenericNotImplementedError<Transform>.TryGet(ladderCheck, core.transform.parent.name);

    /// <summary>
    /// Transform marking the point for platform detection.
    /// </summary>
    [SerializeField, Tooltip("Point from which platform checks originate")]
    private Transform platformCheck;
    public Transform PlatformCheck => GenericNotImplementedError<Transform>.TryGet(platformCheck, core.transform.parent.name);

    #endregion

    #region Check Distances and Offsets

    [Header("Check Distances")]
    [SerializeField, Tooltip("Distance to check for ground below")] 
    private float groundCheckDistance;
    public float GroundCheckDistance { get => groundCheckDistance; set => groundCheckDistance = value; }

    [SerializeField, Tooltip("Horizontal offset for ground check points")] 
    private float groundCheckOffset;
    public float GroundCheckOffset { get => groundCheckOffset; private set => groundCheckOffset = value; }

    [SerializeField, Tooltip("Distance to check for walls")] 
    private float wallCheckDistance;
    public float WallCheckDistance { get => wallCheckDistance; set => wallCheckDistance = value; }

    [Header("Ladder Detection Settings")]
    [Space(10)]
    [SerializeField, Tooltip("Width of the ladder detection box")] 
    private float ladderCheckWidth = 0.3f;
    [SerializeField, Tooltip("Height of the ladder detection box")] 
    private float ladderCheckHeight = 1f;

    [Header("Ladder Continuation Settings")]
    [Space(10)]
    [SerializeField, Tooltip("Radius for checking if player can continue climbing up/down")] 
    private float ladderContinueRadius = 0.15f;
    [SerializeField, Tooltip("How far up to check for ladder continuation")] 
    private float ladderTopDistance = 1f;
    [SerializeField, Tooltip("How far down to check for ladder continuation")] 
    private float ladderBottomDistance = 1f;

    [SerializeField, Tooltip("Distance to check for slopes")] 
    private float slopeCheckDistance;

    [SerializeField, Tooltip("Radius of the platform detection circle")] 
    private float platformCheckRadius;

    #endregion

    #region Collision Layers

    [Header("Layer Masks")]
    [SerializeField, Tooltip("Layers considered as ground")] 
    private LayerMask whatIsGround;
    public LayerMask WhatIsGround { get => whatIsGround; set => whatIsGround = value; }

    [SerializeField, Tooltip("Layers considered as walls")] 
    private LayerMask whatIsWall;
    public LayerMask WhatIsWall { get => whatIsWall; set => whatIsWall = value; }

    [SerializeField, Tooltip("Layers considered as ladders")] 
    private LayerMask whatIsLadder;

    [SerializeField, Tooltip("Layers considered as grippable walls")] 
    private LayerMask whatIsGripWall;
    public LayerMask WhatIsGripWall { get => whatIsGripWall; set => whatIsGripWall = value; }

    [SerializeField, Tooltip("Layers considered as platforms")] 
    private LayerMask whatIsPlatform;
    public LayerMask WhatIsPlatform { get => whatIsPlatform; set => whatIsPlatform = value; }

    [SerializeField, Tooltip("Layers considered as hazards (spikes, etc.)")] 
    private LayerMask whatIsHazard;
    public LayerMask WhatIsHazard { get => whatIsHazard; set => whatIsHazard = value; }

    #endregion

    #region Components

    [SerializeField, Tooltip("Reference to the entity's box collider")] 
    private BoxCollider2D boxCollider2D;
    private Vector2 colliderSize;
    #endregion

    #region Private Fields

    private int raycastDirection;

    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes the component by getting the Core reference.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (boxCollider2D != null)
        {
            colliderSize = boxCollider2D.size;
        }
    }
    #endregion

    #region Collision Checks
    /// <summary>
    /// Checks if the entity is touching the ground using two raycasts.
    /// </summary>
    public bool Ground
    {
        get
        {
            Vector2 leftRaycastOrigin = new Vector2(GroundCheck.position.x - groundCheckOffset, GroundCheck.position.y);
            Vector2 rightRaycastOrigin = new Vector2(GroundCheck.position.x + groundCheckOffset, GroundCheck.position.y);

            bool leftHit = Physics2D.Raycast(leftRaycastOrigin, Vector2.down, groundCheckDistance, whatIsGround);
            bool rightHit = Physics2D.Raycast(rightRaycastOrigin, Vector2.down, groundCheckDistance, whatIsGround);

            return leftHit || rightHit;
        }
    }

    /// <summary>
    /// Checks if there is a wall in front of the entity.
    /// </summary>
    public bool WallFront => Physics2D.Raycast(WallCheck.position, Vector2.right * Movement.FacingDirection, wallCheckDistance, whatIsWall);

    /// <summary>
    /// Checks if there is a wall behind the entity.
    /// </summary>
    public bool WallBack => Physics2D.Raycast(WallCheck.position, Vector2.right * -Movement.FacingDirection, wallCheckDistance, whatIsWall);

    /// <summary>
    /// Checks if there is a grippable wall in front of the entity.
    /// </summary>
    public bool GripWall => Physics2D.Raycast(WallCheck.position, Vector2.right * Movement.FacingDirection, wallCheckDistance, whatIsGripWall);

    /// <summary>
    /// Checks if there is a vertical ledge in front of the entity.
    /// </summary>
    public bool LedgeVertical => Physics2D.Raycast(ledgeCheckVertical.position, Vector2.down, wallCheckDistance, whatIsGround);

    /// <summary>
    /// Checks if there is a horizontal ledge in front of the entity.
    /// </summary>
    public bool LedgeHorizontal => Physics2D.Raycast(LedgeCheckHorizontal.position, Vector2.right * Movement.FacingDirection, wallCheckDistance, whatIsGround);

    /// <summary>
    /// Checks if there is a ladder at the check point using a box check.
    /// </summary>
    public bool Ladder
    {
        get
        {
            Vector2 boxCenter = LadderCheck.position;
            Vector2 boxSize = new Vector2(ladderCheckWidth, ladderCheckHeight);
            return Physics2D.OverlapBox(boxCenter, boxSize, 0f, whatIsLadder);
        }
    }

    /// <summary>
    /// Checks if there is a ladder below the entity.
    /// </summary>
    public bool LadderBottom
    {
        get
        {
            if (!Ladder) return false;

            // Check only at the bottom point
            Vector2 bottomPoint = LadderCheck.position + Vector3.down * ladderBottomDistance;
            return Physics2D.OverlapCircle(bottomPoint, ladderContinueRadius, whatIsLadder);
        }
    }

    /// <summary>
    /// Checks if there is a ladder above the entity.
    /// </summary>
    public bool LadderTop
    {
        get
        {
            if (!Ladder) return false;

            // Check only at the top point
            Vector2 topPoint = LadderCheck.position + Vector3.up * ladderTopDistance;
            return Physics2D.OverlapCircle(topPoint, ladderContinueRadius, whatIsLadder);
        }
    }

    /// <summary>
    /// Checks if there is a platform below the entity.
    /// </summary>
    public bool Platform => Physics2D.OverlapCircle(platformCheck.position, platformCheckRadius, whatIsPlatform);

    /// <summary>
    /// Checks if the entity is currently on a hazard (spikes, etc.).
    /// </summary>
    public bool IsOnHazard
    {
        get
        {
            Vector2 leftRaycastOrigin = new Vector2(GroundCheck.position.x - groundCheckOffset, GroundCheck.position.y);
            Vector2 rightRaycastOrigin = new Vector2(GroundCheck.position.x + groundCheckOffset, GroundCheck.position.y);

            bool leftHit = Physics2D.Raycast(leftRaycastOrigin, Vector2.down, groundCheckDistance, whatIsHazard);
            bool rightHit = Physics2D.Raycast(rightRaycastOrigin, Vector2.down, groundCheckDistance, whatIsHazard);

            return leftHit || rightHit;
        }
    }

    #endregion

    #region Slope Detection

    /// <summary>
    /// Performs slope detection and calculates slope angles.
    /// </summary>
    /// <returns>True if the entity is on a slope, false otherwise.</returns>
    public bool SlopeCheck()
    {
        var isOnSlope = false;

        Vector2 slopeRaycastOrigin = Movement.RB.transform.position - (Vector3)(new Vector2(0.0f, colliderSize.y / 2));
        Vector2 offsetDirection = (Movement != null && Movement.FacingDirection == 1) ? Vector2.right : Vector2.left;
        Vector2 offsetRaycastOrigin = slopeRaycastOrigin + offsetDirection * 0.5f;

        RaycastHit2D slopeHit = Physics2D.Raycast(offsetRaycastOrigin, Vector2.down, slopeCheckDistance, whatIsGround);

        if (slopeHit)
        {
            Vector2 slopeNormalPerp = Vector2.Perpendicular(slopeHit.normal).normalized;

            if (slopeNormalPerp.y > 0.1 || slopeNormalPerp.y < -0.1)
            {
                raycastDirection = slopeNormalPerp.y > 0 ? 1 : -1;
            }
        }

        Vector2 raycastOrigin = Movement.RB.transform.position - (Vector3)(new Vector2(raycastDirection * 0.5f, colliderSize.y / 2));
        RaycastHit2D groundHit = Physics2D.Raycast(raycastOrigin, Vector2.down, slopeCheckDistance, whatIsGround);

        if (groundHit)
        {
            slopeNormalPerp = Vector2.Perpendicular(groundHit.normal).normalized;
            slopeDownAngle = Vector2.Angle(groundHit.normal, Vector2.up);
            isOnSlope = slopeDownAngle != 0.0f;
        }

        return isOnSlope;
    }

    /// <summary>
    /// Gets the position of the ladder the entity is currently on.
    /// </summary>
    /// <returns>The ladder's position, or null if not on a ladder.</returns>
    public Vector3? GetLadderPosition()
    {
        Collider2D collider = Physics2D.OverlapBox(LadderCheck.position, new Vector2(ladderCheckWidth, ladderCheckHeight), 0f, whatIsLadder);
        return collider?.transform.position;
    }

    #endregion

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Draw ladder box check
        Gizmos.color = Color.blue;
        Vector2 boxCenter = LadderCheck.position;
        Vector2 boxSize = new Vector2(ladderCheckWidth, ladderCheckHeight);
        DrawWireBox(boxCenter, boxSize);

        // Draw ladder endpoint checks
        Gizmos.color = Color.cyan;
        // Top check point
        Vector3 topPoint = LadderCheck.position + Vector3.up * ladderTopDistance;
        DrawCircle(topPoint, ladderContinueRadius, 32);
        Gizmos.DrawLine(LadderCheck.position, topPoint);
        
        // Bottom check point
        Vector3 bottomPoint = LadderCheck.position + Vector3.down * ladderBottomDistance;
        DrawCircle(bottomPoint, ladderContinueRadius, 32);
        Gizmos.DrawLine(LadderCheck.position, bottomPoint);

        // Draw ground checks
        Gizmos.color = Color.green;
        Vector2 leftRaycastOrigin = new Vector2(GroundCheck.position.x - groundCheckOffset, GroundCheck.position.y);
        Vector2 rightRaycastOrigin = new Vector2(GroundCheck.position.x + groundCheckOffset, GroundCheck.position.y);
        Gizmos.DrawLine(leftRaycastOrigin, leftRaycastOrigin + Vector2.down * groundCheckDistance);
        Gizmos.DrawLine(rightRaycastOrigin, rightRaycastOrigin + Vector2.down * groundCheckDistance);
    }

    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 lastPoint = center + Vector3.right * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }

    private void DrawWireBox(Vector2 center, Vector2 size)
    {
        Vector2 halfSize = size * 0.5f;
        
        // Draw the box edges
        Vector2 topLeft = center + new Vector2(-halfSize.x, halfSize.y);
        Vector2 topRight = center + new Vector2(halfSize.x, halfSize.y);
        Vector2 bottomLeft = center + new Vector2(-halfSize.x, -halfSize.y);
        Vector2 bottomRight = center + new Vector2(halfSize.x, -halfSize.y);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}