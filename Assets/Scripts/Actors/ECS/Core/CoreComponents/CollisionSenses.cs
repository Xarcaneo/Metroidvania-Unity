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

    [SerializeField, Tooltip("Distance to check for walls")] 
    private float wallCheckDistance;
    public float WallCheckDistance { get => wallCheckDistance; set => wallCheckDistance = value; }

    [SerializeField, Tooltip("Radius of the ladder detection circle")] 
    private float ladderCheckRadius;

    [SerializeField, Tooltip("Distance to check above for ladder top")] 
    private float ladderTopDistance;

    [SerializeField, Tooltip("Distance to check below for ladder bottom")] 
    private float ladderBottomDistance;

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
    /// Checks if there is a ladder at the check point.
    /// </summary>
    public bool Ladder => Physics2D.OverlapCircle(LadderCheck.position, ladderCheckRadius, whatIsLadder);

    /// <summary>
    /// Checks if there is a ladder below the entity.
    /// </summary>
    public bool LadderBottom => Physics2D.OverlapCircle(LadderCheck.position - new Vector3(0, ladderBottomDistance, 0), ladderCheckRadius, whatIsLadder);

    /// <summary>
    /// Checks if there is a ladder above the entity.
    /// </summary>
    public bool LadderTop => Physics2D.OverlapCircle(LadderCheck.position - new Vector3(0, ladderTopDistance, 0), ladderCheckRadius, whatIsLadder);

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
        Collider2D collider = Physics2D.OverlapCircle(LadderCheck.position, ladderCheckRadius, whatIsLadder);
        return collider?.transform.position;
    }

    #endregion
}