using UnityEngine;

public class PlayerLadderClimbState : PlayerState
{
    private int yInput;
    private int xInput;
    private bool JumpInput;

    private bool isGrounded;
    private bool isTouchingLadder;

    private Animator m_anim;
    private Rigidbody2D m_rigidbody;

    private float prevGravityScale;

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    public PlayerLadderClimbState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
        m_anim = player.GetComponent<Animator>();
        m_rigidbody = player.GetComponent<Rigidbody2D>();
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isGrounded = CollisionSenses.Ground;
            isTouchingLadder = collisionSenses.Ladder;
        }
    }

    public override void Enter()
    {
        base.Enter();

        player.JumpState.ResetAmountOfJumpsLeft();

        m_anim.speed = 0;
        prevGravityScale = m_rigidbody.gravityScale;
        m_rigidbody.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();

        m_anim.speed = 1;
        m_rigidbody.gravityScale = prevGravityScale;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);
        Movement?.SetVelocityY(0f);

        yInput = player.InputHandler.NormInputY;
        xInput = player.InputHandler.NormInputX;
        JumpInput = player.InputHandler.JumpInput;

        if (JumpInput && player.JumpState.CanJump() && xInput != 0)
        {
            Movement?.SetVelocityX(0f);
            stateMachine.ChangeState(player.JumpState);
        }
        else if (isGrounded && yInput == -1)
        {
            stateMachine.ChangeState(player.FinishClimb);
        }
        else if(!isTouchingLadder)
        {
            stateMachine.ChangeState(player.InAirState);
        }
        else if (yInput != 0)
        {
            m_anim.speed = 1;
            Movement?.SetVelocityY(playerData.climbingVelocity * yInput);;
        }
        else
            m_anim.speed = 0;

        if (yInput == 1 && playerData.climbFinishThresholdUp >= DetermineDistanceFromGround(true))
        {
            player.transform.position = DetermineGroundPosition();
            stateMachine.ChangeState(player.FinishClimb);
        }
    }

    private Vector2 DetermineGroundPosition()
    {
        // Cast a ray upwards to find the ground above the ladder
        RaycastHit2D hit = Physics2D.Raycast(CollisionSenses.GroundCheck.position, Vector2.up, playerData.ladderTopRaycastLength, CollisionSenses.WhatIsGround);

        // Set the player's position to be just above where the raycast hit the ground
        return new Vector2(player.transform.position.x, hit.point.y + playerData.groundOffset);
    }

    private float DetermineDistanceFromGround(bool checkAbove)
    {
        // Determine the direction based on whether we're checking above or below the player
        Vector2 raycastDirection = checkAbove ? Vector2.up : Vector2.down;
        // Start the raycast from the player's position
        Vector2 raycastStartPoint = new Vector2(player.transform.position.x, player.transform.position.y);

        // Cast a ray in the determined direction to find the ground
        RaycastHit2D hit = Physics2D.Raycast(raycastStartPoint, raycastDirection, Mathf.Infinity, CollisionSenses.WhatIsGround);

        // If we hit something, calculate the distance, otherwise return a default large distance
        if (hit.collider != null)
        {
            // Return the distance from the player to the ground
            return Mathf.Abs(hit.point.y - raycastStartPoint.y);
        }
        else
        {
            // Return a large value if there's no ground detected in the chosen direction
            return float.MaxValue;
        }
    }
}
