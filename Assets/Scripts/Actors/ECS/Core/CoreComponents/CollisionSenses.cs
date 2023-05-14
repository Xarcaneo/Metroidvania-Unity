using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSenses : CoreComponent
{
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

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

    public float GroundCheckRadius { get => groundCheckRadius; set => groundCheckRadius = value; }
    public float WallCheckDistance { get => wallCheckDistance; set => wallCheckDistance = value; }
    public LayerMask WhatIsGround { get => whatIsGround; set => whatIsGround = value; }
    public LayerMask WhatIsWall { get => whatIsWall; set => whatIsWall = value; }

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheckHorizontal;
    [SerializeField] private Transform ledgeCheckVertical;
    [SerializeField] private Transform ladderCheck;

    [SerializeField] private float groundCheckRadius;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float ladderCheckRadius;
    [SerializeField] private float ladderTopDistance;
    [SerializeField] private float ladderBottomDistance;

    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private LayerMask whatIsLadder;
    #endregion

    public bool Ground
    {
        get => Physics2D.OverlapBox(GroundCheck.position, new Vector2(groundCheckRadius * 2, 0.1f), 0, whatIsGround);
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
}