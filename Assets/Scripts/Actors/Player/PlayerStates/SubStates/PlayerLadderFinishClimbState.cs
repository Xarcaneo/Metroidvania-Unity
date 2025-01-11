using UnityEngine;

/// <summary>
/// State that handles the completion of ladder climbing.
/// This state manages the animation and positioning when reaching the top of a ladder.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Playing the ladder climb completion animation
/// - Calculating and setting proper climb positions
/// - Ensuring proper alignment with ladder and platform
/// - Transitioning to idle state when complete
/// 
/// The state automatically transitions to:
/// - IdleState: When the climbing finish animation is complete
/// </remarks>
public class PlayerLadderFinishClimbState : PlayerAbilityState
{
    #region Position Variables
    /// <summary>
    /// Position of the ladder top being climbed
    /// </summary>
    private Vector2 ladderTopPos;

    /// <summary>
    /// Starting position for the climb animation
    /// </summary>
    private Vector2 startPos;

    /// <summary>
    /// Final position after climbing
    /// </summary>
    private Vector2 stopPos;
    #endregion

    #region Core Components
    /// <summary>
    /// Reference to the Movement component is inherited from PlayerAbilityState
    /// </summary>
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerLadderFinishClimbState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerLadderFinishClimbState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Called when entering the ladder finish climb state
    /// </summary>
    /// <remarks>
    /// Sets up the finish climb by:
    /// 1. Calling base class Enter method
    /// 2. Calculating climb positions
    /// 3. Setting initial position for animation
    /// </remarks>
    public override void Enter()
    {
        base.Enter();

        // Calculate ladder top position
        ladderTopPos = CalculateLadderTopPosition();

        // Calculate start and stop positions for climbing
        startPos = new Vector2(ladderTopPos.x, ladderTopPos.y - playerData.startOffset.y);
        stopPos = new Vector2(ladderTopPos.x, ladderTopPos.y + playerData.stopOffset.y);

        // Ensure we're at the correct starting position and stop movement
        Movement?.SetVelocityZero();
        player.transform.position = startPos;

        // Ensure we're properly aligned with the ladder
        AlignWithLadder();
    }

    private void AlignWithLadder()
    {
        // Cast rays to find the edges of the platform
        float rayLength = 1f;
        RaycastHit2D leftHit = Physics2D.Raycast(
            startPos + Vector2.left * 0.5f + Vector2.up * 0.5f, 
            Vector2.right, 
            rayLength, 
            CollisionSenses.WhatIsGround
        );

        RaycastHit2D rightHit = Physics2D.Raycast(
            startPos + Vector2.right * 0.5f + Vector2.up * 0.5f, 
            Vector2.left, 
            rayLength, 
            CollisionSenses.WhatIsGround
        );

        // If we found both edges, center between them
        if (leftHit.collider != null && rightHit.collider != null)
        {
            float center = (leftHit.point.x + rightHit.point.x) / 2f;
            startPos = new Vector2(center, startPos.y);
            stopPos = new Vector2(center, stopPos.y);
            player.transform.position = startPos;
        }
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Keep player at correct position during animation
    /// 2. Check for animation completion
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Keep player at the correct position during animation
        Movement?.SetVelocityZero();
        
        if (isAnimationFinished)
        {
            player.transform.position = stopPos;
            isAbilityDone = true;
        }

        if (isAbilityDone)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    /// <summary>
    /// Called when exiting the ladder finish climb state
    /// </summary>
    /// <remarks>
    /// Ensures proper final positioning before transitioning
    /// </remarks>
    public override void Exit()
    {
        base.Exit();
        player.transform.position = stopPos;
    }

    /// <summary>
    /// Calculates the position of the ladder top for proper alignment
    /// </summary>
    /// <returns>The position of the ladder top</returns>
    private Vector2 CalculateLadderTopPosition()
    {
        // Cast a ray upward to find the ground above the ladder
        RaycastHit2D hit = Physics2D.Raycast(
            player.transform.position,
            Vector2.up,
            playerData.ladderTopRaycastLength,
            CollisionSenses.WhatIsGround
        );

        if (hit.collider != null)
        {
            // Return a position just below the detected ground
            return new Vector2(player.transform.position.x, hit.point.y);
        }

        // Fallback if no ground is detected
        return player.transform.position;
    }
}
