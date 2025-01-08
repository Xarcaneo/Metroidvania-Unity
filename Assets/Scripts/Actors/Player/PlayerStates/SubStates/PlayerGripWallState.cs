using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State that handles the player's wall grip mechanics.
/// This state allows the player to grip onto walls and perform wall jumps.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Maintaining the player's position while gripping a wall
/// - Managing wall jump timing and mechanics
/// - Handling vertical movement inputs while gripped
/// 
/// The state automatically transitions to:
/// - WallJumpState: When jump input is detected and wall jump is available
/// - InAirState: When the player inputs downward direction
/// </remarks>
public class PlayerGripWallState : PlayerState
{
    #region State Variables
    /// <summary>
    /// The position where the player grips the wall
    /// </summary>
    private Vector2 gripPos;

    /// <summary>
    /// Time when the player started touching the wall
    /// </summary>
    /// <remarks>
    /// Used to calculate wall jump availability based on playerData.wallTouchTime
    /// </remarks>
    private float touchingWallTime = 0.0f;

    /// <summary>
    /// Flag indicating if wall jump is currently available
    /// </summary>
    private bool canWallJump = false;

    /// <summary>
    /// Flag indicating if the player is touching a wall
    /// </summary>
    private bool isTouchingWall;
    #endregion

    #region Input Variables
    /// <summary>
    /// Flag indicating if jump input has been detected
    /// </summary>
    private bool jumpInput;

    /// <summary>
    /// Vertical input value (-1 for down, 0 for neutral, 1 for up)
    /// </summary>
    private int yInput;
    #endregion

    #region Core Components
    /// <summary>
    /// Reference to the Movement component, lazily loaded
    /// </summary>
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    /// <summary>
    /// Reference to the CollisionSenses component, lazily loaded
    /// </summary>
    protected CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerGripWallState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerGripWallState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Performs state-specific checks
    /// </summary>
    /// <remarks>
    /// Updates wall contact status by checking if the player is touching a wall
    /// in front of them using CollisionSenses.
    /// </remarks>
    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isTouchingWall = CollisionSenses.WallFront;
        }
    }

    /// <summary>
    /// Called when entering the wall grip state
    /// </summary>
    /// <remarks>
    /// Sets up the wall grip by:
    /// 1. Calling base class Enter method
    /// 2. Recording the time wall contact began
    /// 3. Storing the initial grip position
    /// </remarks>
    public override void Enter()
    {
        base.Enter();

        touchingWallTime = Time.time;
        gripPos = player.transform.position;
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Update wall jump availability
    /// 2. Process player inputs
    /// 3. Maintain grip position
    /// 4. Handle state transitions
    /// 
    /// The state will transition to:
    /// - WallJumpState: When jump input is detected and wall jump is available
    /// - InAirState: When downward input is detected
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        CheckIfCanWallJump();

        yInput = player.InputHandler.NormInputY;
        jumpInput = player.InputHandler.JumpInput;

        if (!isExitingState)
        {
            // Keep player fixed at grip position
            Movement?.SetVelocityZero();
            player.transform.position = gripPos;
        }

        if (isAnimationFinished)
        {
            if (jumpInput && canWallJump)
            {
                // Prepare wall jump direction and transition to wall jump state
                player.WallJumpState.DetermineWallJumpDirection(isTouchingWall);
                stateMachine.ChangeState(player.WallJumpState);
            }
            else if (yInput == -1)
            {
                // Release grip when player inputs downward direction
                stateMachine.ChangeState(player.InAirState);
            }
        }
    }

    /// <summary>
    /// Called when exiting the wall grip state
    /// </summary>
    /// <remarks>
    /// Cleans up the wall grip state by:
    /// 1. Calling base class Exit method
    /// 2. Consuming the block input to prevent immediate re-entry
    /// </remarks>
    public override void Exit()
    {
        base.Exit();

        player.InputHandler.UseBlockInput();
    }

    /// <summary>
    /// Determines if wall jump is currently available
    /// </summary>
    /// <remarks>
    /// Wall jump becomes available after the player has been touching
    /// the wall for longer than playerData.wallTouchTime.
    /// </remarks>
    private void CheckIfCanWallJump()
    {
        canWallJump = Time.time > touchingWallTime + playerData.wallTouchTime;
    }
}
