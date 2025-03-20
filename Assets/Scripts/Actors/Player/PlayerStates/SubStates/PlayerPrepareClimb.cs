using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State that handles the preparation phase before ladder climbing.
/// This state manages the transition animation and positioning before actual climbing begins.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing the pre-climb animation
/// - Adjusting player position for ladder entry
/// - Ensuring smooth transition to climbing state
/// 
/// The state can transition to:
/// - LadderClimbState: When preparation animation finishes
/// </remarks>
public class PlayerPrepareClimb : PlayerState
{
    #region State Variables
    /// <summary>
    /// Direction of climbing (-1 for down, 0 for none, 1 for up)
    /// </summary>
    private int climbingDirection;

    private bool isAligningDown = false;
    private float targetYPosition;
    private float startYPosition;
    private float alignmentProgress = 0f;
    #endregion

    #region Core Components
    /// <summary>
    /// Reference to the Movement component, lazily loaded
    /// </summary>
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerPrepareClimb
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerPrepareClimb(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Called when entering the prepare climb state
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        // Only interpolate Y position when climbing down from ground
        if (climbingDirection == -1)
        {
            isAligningDown = true;
            alignmentProgress = 0f;
            startYPosition = player.transform.position.y;
            // Move down by 1 unit to get onto ladder
            targetYPosition = startYPosition - playerData.startPosDown;
        }
    }

    /// <summary>
    /// Called when exiting the prepare climb state
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        // Only adjust position when climbing up
        if (climbingDirection == 1)
            Player.Instance.gameObject.transform.position += new Vector3(0, playerData.startPosUp, 0);

        climbingDirection = 0;
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Handle Y position interpolation when climbing down
        if (isAligningDown)
        {
            alignmentProgress += Time.deltaTime * playerData.climbDownInterpolationSpeed;
            
            // Get ladder X position for alignment
            Vector3? ladderPosition = player.LadderClimbState.GetLadderPositionWithWideScan();
            float newY = Mathf.Lerp(startYPosition, targetYPosition, alignmentProgress);
            
            // Update both X and Y position
            if (ladderPosition.HasValue)
            {
                player.transform.position = new Vector3(ladderPosition.Value.x, newY, player.transform.position.z);
            }

            // Check if we're done aligning
            if (alignmentProgress >= 1f)
            {
                isAligningDown = false;
                player.transform.position = new Vector3(player.transform.position.x, targetYPosition, player.transform.position.z);
                stateMachine.ChangeState(player.LadderClimbState);
                return;
            }
        }
        else
        {
            // Ensure player is stationary during preparation
            Movement?.SetVelocityX(0f);
            Movement?.SetVelocityY(0f);

            // Transition to climb state when preparation is complete
            if (isAnimationFinished)
            {
                stateMachine.ChangeState(player.LadderClimbState);
            }
        }
    }

    /// <summary>
    /// Sets the direction for climbing
    /// </summary>
    /// <param name="direction">The direction to climb (-1 for down, 1 for up)</param>
    /// <remarks>
    /// Called before entering the state to determine the correct positioning adjustment
    /// </remarks>
    public void SetClimbingDirection(int direction) => climbingDirection = direction;
}
