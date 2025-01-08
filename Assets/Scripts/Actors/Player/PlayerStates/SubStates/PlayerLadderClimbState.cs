using UnityEngine;

/// <summary>
/// State that handles the player's ladder climbing behavior.
/// This state manages climbing up and down ladders with smooth transitions.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing ladder climbing movement
/// - Handling ladder animation speed
/// - Controlling gravity during climbing
/// - Managing transitions at ladder ends
/// - Aligning player with ladder center
/// 
/// The state can transition to:
/// - JumpState: When jumping off ladder with horizontal input
/// - FinishClimb: When reaching ladder end or pressing down at bottom
/// - InAirState: When losing contact with ladder
/// </remarks>
public class PlayerLadderClimbState : PlayerState
{
    #region Input Variables
    /// <summary>
    /// Vertical input value (-1 for down, 0 for neutral, 1 for up)
    /// </summary>
    private int yInput;

    /// <summary>
    /// Horizontal input value (-1 for left, 0 for neutral, 1 for right)
    /// </summary>
    private int xInput;

    /// <summary>
    /// Flag indicating if jump input is active
    /// </summary>
    private bool JumpInput;
    #endregion

    #region Check Variables
    /// <summary>
    /// Flag indicating if player is touching ground
    /// </summary>
    private bool isGrounded;

    /// <summary>
    /// Flag indicating if player is touching ladder
    /// </summary>
    private bool isTouchingLadder;
    #endregion

    #region Components
    /// <summary>
    /// Reference to the player's animator component
    /// </summary>
    private Animator m_anim;

    /// <summary>
    /// Reference to the player's rigidbody component
    /// </summary>
    private Rigidbody2D m_rigidbody;
    #endregion

    #region State Variables
    /// <summary>
    /// Stores the original gravity scale to restore after climbing
    /// </summary>
    private float prevGravityScale;
    #endregion

    #region Core Components
    /// <summary>
    /// Reference to the Movement component, lazily loaded
    /// </summary>
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    /// <summary>
    /// Reference to the CollisionSenses component, lazily loaded
    /// </summary>
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerLadderClimbState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerLadderClimbState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
        m_anim = player.GetComponent<Animator>();
        m_rigidbody = player.GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Performs state-specific checks
    /// </summary>
    /// <remarks>
    /// Updates ground contact and ladder contact status
    /// </remarks>
    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isGrounded = CollisionSenses.Ground;
            isTouchingLadder = collisionSenses.Ladder;
        }
    }

    /// <summary>
    /// Called when entering the ladder climb state
    /// </summary>
    /// <remarks>
    /// Sets up climbing by:
    /// 1. Resetting available jumps
    /// 2. Pausing climb animation
    /// 3. Disabling gravity
    /// 4. Aligning player with ladder center
    /// </remarks>
    public override void Enter()
    {
        base.Enter();

        player.JumpState.ResetAmountOfJumpsLeft();

        m_anim.speed = 0;
        prevGravityScale = m_rigidbody.gravityScale;
        m_rigidbody.gravityScale = 0;

        AlignPlayerWithLadderCenter();
    }

    /// <summary>
    /// Called when exiting the ladder climb state
    /// </summary>
    /// <remarks>
    /// Restores normal movement by:
    /// 1. Resuming normal animation speed
    /// 2. Restoring original gravity scale
    /// </remarks>
    public override void Exit()
    {
        base.Exit();

        m_anim.speed = 1;
        m_rigidbody.gravityScale = prevGravityScale;
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Stop any horizontal movement
    /// 2. Process player inputs
    /// 3. Handle state transitions
    /// 4. Update climbing movement and animation
    /// 5. Check for ladder end points
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);
        Movement?.SetVelocityY(0f);

        yInput = player.InputHandler.NormInputY;
        xInput = player.InputHandler.NormInputX;
        JumpInput = player.InputHandler.JumpInput;

        // Handle state transitions
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
            // Update climbing animation and movement
            m_anim.speed = 1;
            Movement?.SetVelocityY(playerData.climbingVelocity * yInput);
        }
        else
            m_anim.speed = 0;

        // Check if player has reached ladder top
        if (yInput == 1 && playerData.climbFinishThresholdUp >= DetermineDistanceFromGround(true))
        {
            player.transform.position = DetermineGroundPosition();
            stateMachine.ChangeState(player.FinishClimb);
        }
    }

    /// <summary>
    /// Determines the position where the player should be placed when reaching ladder top
    /// </summary>
    /// <returns>Vector2 position above the ladder</returns>
    /// <remarks>
    /// Casts a ray upward to find the ground position and adds an offset
    /// </remarks>
    private Vector2 DetermineGroundPosition()
    {
        // Cast a ray upwards to find the ground above the ladder
        RaycastHit2D hit = Physics2D.Raycast(CollisionSenses.GroundCheck.position, Vector2.up, playerData.ladderTopRaycastLength, CollisionSenses.WhatIsGround);

        // Set the player's position to be just above where the raycast hit the ground
        return new Vector2(player.transform.position.x, hit.point.y + playerData.groundOffset);
    }

    /// <summary>
    /// Determines the distance to the nearest ground above or below the player
    /// </summary>
    /// <param name="checkAbove">If true, checks above player; if false, checks below</param>
    /// <returns>Distance to the nearest ground in the specified direction</returns>
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

    /// <summary>
    /// Aligns the player horizontally with the center of the ladder
    /// </summary>
    /// <remarks>
    /// Uses the ladder position from CollisionSenses to ensure proper alignment
    /// </remarks>
    private void AlignPlayerWithLadderCenter()
    {
        if (CollisionSenses)
        {
            Vector3? ladderPosition = CollisionSenses.GetLadderPosition();
            if (ladderPosition.HasValue)
            {
                // Correctly use the .Value property to access the Vector3 inside the nullable Vector3?
                Vector3 newPosition = new Vector3(ladderPosition.Value.x, player.transform.position.y, player.transform.position.z);
                player.transform.position = newPosition;
            }
        }
    }
}
