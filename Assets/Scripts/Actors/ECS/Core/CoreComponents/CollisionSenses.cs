using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSenses : CoreComponent
{
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    public float slopeDownAngle;
    public Vector2 slopeNormalPerp;

    #region Check Transforms

    public Transform GroundCheck
    {
        get => GenericNotImplementedError<Transform>.TryGet(groundCheck, core.transform.parent.name);
        private set => groundCheck = value;
    }
    public Transform WallCheck
    {
        get => GenericNotImplementedError<Transform>.TryGet(wallCheck, core.transform.parent.name);
        private set => wallCheck = value;
    }
    public Transform LedgeCheckHorizontal
    {
        get => GenericNotImplementedError<Transform>.TryGet(ledgeCheckHorizontal, core.transform.parent.name);
        private set => ledgeCheckHorizontal = value;
    }
    public Transform LedgeCheckVertical
    {
        get => GenericNotImplementedError<Transform>.TryGet(ledgeCheckVertical, core.transform.parent.name);
        private set => ledgeCheckVertical = value;
    }

    public Transform LadderCheck
    {
        get => GenericNotImplementedError<Transform>.TryGet(ladderCheck, core.transform.parent.name);
        private set => ladderCheck = value;
    }

    public float GroundCheckDistance { get => groundCheckDistance; set => groundCheckDistance = value; }
    public float WallCheckDistance { get => wallCheckDistance; set => wallCheckDistance = value; }
    public LayerMask WhatIsGround { get => whatIsGround; set => whatIsGround = value; }
    public LayerMask WhatIsWall { get => whatIsWall; set => whatIsWall = value; }

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheckHorizontal;
    [SerializeField] private Transform ledgeCheckVertical;
    [SerializeField] private Transform ladderCheck;

    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float groundCheckOffset;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float ladderCheckRadius;
    [SerializeField] private float ladderTopDistance;
    [SerializeField] private float ladderBottomDistance;
    [SerializeField] private float slopeCheckDistance;
    [SerializeField] private BoxCollider2D boxCollider2D;

    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private LayerMask whatIsLadder;

    private int raycastDirection;
    #endregion

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

    public bool WallFront
    {
        get => Physics2D.Raycast(WallCheck.position, Vector2.right * Movement.FacingDirection, wallCheckDistance, whatIsWall);
    }

    public bool WallBack
    {
        get => Physics2D.Raycast(WallCheck.position, Vector2.right * -Movement.FacingDirection, wallCheckDistance, whatIsWall);
    }
    public bool LedgeVertical
    {
        get => Physics2D.Raycast(ledgeCheckVertical.position, Vector2.down, wallCheckDistance, whatIsGround);
    }
    public bool LedgeHorizontal
    {
        get => Physics2D.Raycast(LedgeCheckHorizontal.position, Vector2.right * Movement.FacingDirection, wallCheckDistance, whatIsGround);
    }
    public bool Ladder
    {
        get => Physics2D.OverlapCircle(LadderCheck.position, ladderCheckRadius, whatIsLadder);
    }
    public bool LadderBottom
    {
        get => Physics2D.OverlapCircle(LadderCheck.position - new Vector3(0, ladderBottomDistance, 0), ladderCheckRadius, whatIsLadder);
    }

    public bool LadderTop
    {
        get => Physics2D.OverlapCircle(LadderCheck.position - new Vector3(0, ladderTopDistance, 0), ladderCheckRadius, whatIsLadder);
    }

    public bool SlopeCheck()
    {
        var isOnSlope = false;

        Vector2 slopeRaycastOrigin = Movement.RB.transform.position - (Vector3)(new Vector2(0.0f, boxCollider2D.size.y / 2));

        // Offset the raycast position based on player's facing direction
        Vector2 offsetDirection = (Movement != null && Movement.FacingDirection == 1) ? Vector2.right : Vector2.left;
        Vector2 offsetRaycastOrigin = slopeRaycastOrigin + offsetDirection * 0.5f;

        // Perform the second raycast to check for slope
        RaycastHit2D slopeHit = Physics2D.Raycast(offsetRaycastOrigin, Vector2.down, slopeCheckDistance, whatIsGround);


        if (slopeHit)
        {
            Vector2 slopeNormalPerp = Vector2.Perpendicular(slopeHit.normal).normalized;
            //Debug.DrawRay(slopeHit.point, slopeNormalPerp, Color.yellow);

            if (slopeNormalPerp.y > 0.1 || slopeNormalPerp.y < -0.1)
            {
                if (slopeNormalPerp.y > 0)
                    raycastDirection = 1;
                else if (slopeNormalPerp.y < 0)
                    raycastDirection = -1;
            }
        }

        Vector2 raycastOrigin = Movement.RB.transform.position - (Vector3)(new Vector2(raycastDirection * 0.5f, boxCollider2D.size.y / 2));
        RaycastHit2D groundHit = Physics2D.Raycast(raycastOrigin, Vector2.down, slopeCheckDistance, whatIsGround);

        if (groundHit)
        {
            slopeNormalPerp = Vector2.Perpendicular(groundHit.normal).normalized;

            slopeDownAngle = Vector2.Angle(groundHit.normal, Vector2.up);
 
            //Debug.DrawRay(groundHit.point, slopeNormalPerp, Color.blue);
            //Debug.DrawRay(groundHit.point, groundHit.normal, Color.green);
 
            if (slopeDownAngle != 0.0)
            {
                isOnSlope = true;
            }
        }

        return isOnSlope;
    }
}