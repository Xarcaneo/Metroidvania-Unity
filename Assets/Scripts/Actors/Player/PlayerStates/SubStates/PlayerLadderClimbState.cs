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

    private float yInputFloat;
    private float xInputFloat;
    private bool JumpInputFloat;
    private static float lastLadderFinishTime;

    public bool CanEnterLadder() => Time.time - lastLadderFinishTime >= playerData.ladderClimbCooldown;
    public void StartCooldown() => lastLadderFinishTime = Time.time;
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
    public override void Enter()
    {
        base.Enter();

        player.JumpState.ResetAmountOfJumpsLeft();

        m_anim.speed = 0;
        prevGravityScale = m_rigidbody.gravityScale;
        m_rigidbody.gravityScale = 0;

        // Instant X alignment for ladder grab
        Vector3? ladderPosition = GetLadderPositionWithWideScan();
        if (ladderPosition.HasValue)
        {
            Vector3 newPosition = new Vector3(ladderPosition.Value.x, player.transform.position.y, player.transform.position.z);
            player.transform.position = newPosition;
        }
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

        yInput = player.InputHandler.NormInputY;
        xInput = player.InputHandler.NormInputX;
        JumpInput = player.InputHandler.JumpInput;

        // Handle state transitions
        if (JumpInput && player.JumpState.CanJump() && xInput != 0)
        {
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
        else
        {
            // Update climbing animation and movement based on input
            if (yInput != 0)
            {
                m_anim.speed = 1;
                
                // Check for top platform when moving up
                if (yInput == 1)
                {
                    float distanceToTop = DetermineDistanceFromGround(true);
                    
                    if (distanceToTop <= playerData.climbFinishThresholdUp)
                    {
                        player.transform.position = new Vector2(
                            player.transform.position.x,
                            player.transform.position.y + distanceToTop - 0.1f);
                        stateMachine.ChangeState(player.LadderFinishClimbState);
                        return;
                    }
                }
                
                Movement?.SetVelocityY(playerData.climbingVelocity * yInput);
            }
            else
            {
                m_anim.speed = 0;
                Movement?.SetVelocityY(0f);
            }
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
        Vector2 raycastDirection = checkAbove ? Vector2.up : Vector2.down;
        
        // Use multiple raycasts for better detection
        Vector2 raycastStartPoint = player.transform.position;
        float raySpacing = 0.2f; // Space between rays
        int numRays = 3; // Number of rays to cast
        
        float minDistance = float.MaxValue;
        
        for (int i = 0; i < numRays; i++)
        {
            // Offset the ray start points left and right of the player
            float xOffset = (i - (numRays - 1) / 2f) * raySpacing;
            Vector2 startPoint = raycastStartPoint + new Vector2(xOffset, 0);
            
            RaycastHit2D hit = Physics2D.Raycast(startPoint, raycastDirection, 
                playerData.ladderTopRaycastLength, CollisionSenses.WhatIsGround);
            
            if (hit.collider != null)
            {
                float distance = Mathf.Abs(hit.point.y - raycastStartPoint.y);
                minDistance = Mathf.Min(minDistance, distance);
            }
        }
        
        return minDistance;
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
            // Try to get ladder position using multiple check points
            Vector3? ladderPosition = GetLadderPositionWithWideScan();
            
            if (ladderPosition.HasValue)
            {
                Vector3 newPosition = new Vector3(ladderPosition.Value.x, player.transform.position.y, player.transform.position.z);
                player.transform.position = newPosition;
            }
            else
            {
                Debug.LogWarning("[LadderClimbState] Failed to get ladder position!");
            }
        }
    }

    public Vector3? GetLadderPositionWithWideScan()
    {
        // First try the normal ladder check
        Vector3? normalCheck = CollisionSenses.GetLadderPosition();
        if (normalCheck.HasValue) return normalCheck;

        // If normal check fails, do a wider scan
        float scanWidth = 0.5f;  // How far to scan left and right
        float scanStep = 0.25f;  // Distance between scan points
        Vector3 basePosition = player.transform.position;

        for (float offset = -scanWidth; offset <= scanWidth; offset += scanStep)
        {
            Vector2 checkPoint = new Vector2(basePosition.x + offset, basePosition.y - 0.5f);
            Collider2D collider = Physics2D.OverlapCircle(checkPoint, 0.1f, LayerMask.GetMask("Ladder"));
            if (collider != null)
            {
                return collider.transform.position;
            }
        }

        return null;
    }
}
