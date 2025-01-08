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
    /// Called when exiting the prepare climb state
    /// </summary>
    /// <remarks>
    /// Adjusts the player's position based on climbing direction:
    /// - Moves up if entering ladder from bottom
    /// - Moves down if entering ladder from top
    /// - Resets climbing direction
    /// </remarks>
    public override void Exit()
    {
        base.Exit();

        // Adjust position based on climbing direction
        if (climbingDirection == 1)
            Player.Instance.gameObject.transform.position += new Vector3(0, playerData.startPosUp, 0);
        else if (climbingDirection == -1)
            Player.Instance.gameObject.transform.position -= new Vector3(0, playerData.startPosDown, 0);

        climbingDirection = 0;
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Stop all movement during preparation
    /// 2. Check for animation completion
    /// 3. Transition to ladder climb state when ready
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Ensure player is stationary during preparation
        Movement?.SetVelocityX(0f);
        Movement?.SetVelocityY(0f);

        // Transition to climb state when preparation is complete
        if (isAnimationFinished)
        {
            stateMachine.ChangeState(player.LadderClimbState);
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
