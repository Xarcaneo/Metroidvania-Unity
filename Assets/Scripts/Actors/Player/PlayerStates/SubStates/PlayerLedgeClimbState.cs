using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State that handles the player's ledge climbing behavior.
/// This state manages the detection, positioning, and animation of ledge climbing.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Detecting and calculating ledge positions
/// - Managing climb animation and positioning
/// - Handling player input during climbing
/// 
/// The state can transition to:
/// - IdleState: When climbing is complete
/// - InAirState: When player chooses to drop from ledge
/// </remarks>
public class PlayerLedgeClimbState : PlayerState
{
    #region Position Variables
    /// <summary>
    /// Position where the ledge was initially detected
    /// </summary>
    private Vector2 detectedPos;

    /// <summary>
    /// Position of the corner being climbed
    /// </summary>
    private Vector2 cornerPos;

    /// <summary>
    /// Starting position for the climb animation
    /// </summary>
    private Vector2 startPos;

    /// <summary>
    /// Final position after climbing
    /// </summary>
    private Vector2 stopPos;

    /// <summary>
    /// Temporary workspace for vector calculations
    /// </summary>
    private Vector2 workspace;
    #endregion

    #region State Variables
    /// <summary>
    /// Whether the player is currently hanging on the ledge
    /// </summary>
    private bool isHanging;

    /// <summary>
    /// Whether the player is actively climbing
    /// </summary>
    private bool isClimbing;

    /// <summary>
    /// Vertical input value (-1 to 1)
    /// </summary>
    private int yInput;
    #endregion

    #region Core Components
    /// <summary>
    /// Reference to the Movement component, lazily loaded
    /// </summary>
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }

    /// <summary>
    /// Reference to the CollisionSenses component, lazily loaded
    /// </summary>
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }

    private Movement movement;
    private CollisionSenses collisionSenses;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerLedgeClimbState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerLedgeClimbState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Called when the climb animation finishes
    /// </summary>
    /// <remarks>
    /// Resets the climb animation flag
    /// </remarks>
    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        player.Anim.SetBool("climbLedge", false);
    }

    /// <summary>
    /// Called during animation events
    /// </summary>
    /// <remarks>
    /// Sets the hanging flag when triggered by animation
    /// </remarks>
    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
        isHanging = true;
    }

    /// <summary>
    /// Called when entering the ledge climb state
    /// </summary>
    /// <remarks>
    /// Initializes climbing by:
    /// 1. Stopping movement
    /// 2. Positioning player at detected ledge position
    /// 3. Calculating corner and climb positions
    /// </remarks>
    public override void Enter()
    {
        base.Enter();
  
        // Stop all movement
        Movement?.SetVelocityZero();

        // Position player at detected ledge position
        player.transform.position = detectedPos;
        cornerPos = DetermineCornerPosition();

        // Calculate start and stop positions for climbing
        startPos.Set(cornerPos.x - (Movement.FacingDirection * playerData.startOffset.x), 
                    cornerPos.y - playerData.startOffset.y);
        stopPos.Set(cornerPos.x + (Movement.FacingDirection * playerData.stopOffset.x), 
                   cornerPos.y + playerData.stopOffset.y);

        player.transform.position = startPos;
    }

    /// <summary>
    /// Called when exiting the ledge climb state
    /// </summary>
    /// <remarks>
    /// Cleans up climbing state and ensures proper final positioning
    /// </remarks>
    public override void Exit()
    {
        base.Exit();

        isHanging = false;

        if (isClimbing)
        {
            Movement?.SetVelocityY(0);
            player.transform.position = stopPos;
            isClimbing = false;
        }
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Check for animation completion
    /// 2. Handle player input during hanging
    /// 3. Manage climbing transitions
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            stateMachine.ChangeState(player.IdleState);
        }
        else
        {
            // Get vertical input
            yInput = player.InputHandler.NormInputY;

            // Ensure player stays in position during climb
            Movement?.SetVelocityZero();
            player.transform.position = startPos;

            // Handle climbing input
            if (yInput == 1 && isHanging && !isClimbing)
            {
                isClimbing = true;
                player.Anim.SetBool("climbLedge", true);
            }
            else if (yInput == -1 && isHanging && !isClimbing)
            {
                stateMachine.ChangeState(player.InAirState);
            }
        }
    }

    /// <summary>
    /// Sets the position where the ledge was detected
    /// </summary>
    /// <param name="pos">The detected ledge position</param>
    public void SetDetectedPosition(Vector2 pos) => detectedPos = pos;

    /// <summary>
    /// Calculates the exact corner position for climbing
    /// </summary>
    /// <returns>The Vector2 position of the corner to climb</returns>
    /// <remarks>
    /// Uses raycasts to:
    /// 1. Find the exact wall position
    /// 2. Find the exact ground position
    /// 3. Calculate the corner from these positions
    /// </remarks>
    private Vector2 DetermineCornerPosition()
    {
        // Cast ray to find wall position
        RaycastHit2D xHit = Physics2D.Raycast(CollisionSenses.WallCheck.position, 
            Vector2.right * Movement.FacingDirection, 
            CollisionSenses.WallCheckDistance, 
            CollisionSenses.WhatIsGround);
        float xDist = xHit.distance;

        // Calculate position for vertical raycast
        workspace.Set((xDist + 0.015f) * Movement.FacingDirection, 0f);

        // Cast ray to find ground position
        RaycastHit2D yHit = Physics2D.Raycast(
            CollisionSenses.LedgeCheckHorizontal.position + (Vector3)(workspace), 
            Vector2.down, 
            CollisionSenses.LedgeCheckHorizontal.position.y - CollisionSenses.WallCheck.position.y + 0.015f, 
            CollisionSenses.WhatIsGround);
        float yDist = yHit.distance;

        // Calculate final corner position
        workspace.Set(
            CollisionSenses.WallCheck.position.x + (xDist * Movement.FacingDirection), 
            CollisionSenses.LedgeCheckHorizontal.position.y - yDist);
        return workspace;
    }
}