using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State that handles the player's idle behavior.
/// This state represents when the player is standing still on the ground.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Maintaining zero horizontal velocity
/// - Handling transitions to other states based on input
/// - Managing ladder climb and crouch transitions
/// 
/// The state can transition to:
/// - MoveState: When horizontal input is detected
/// - PrepareClimb: When vertical input is detected near a ladder
/// - CrouchIdleState: When pressing down without a ladder
/// </remarks>
public class PlayerIdleState : PlayerGroundedState
{
    /// <summary>
    /// Time delay in seconds before allowing ladder interaction after entering idle state
    /// </summary>
    /// <remarks>
    /// This delay prevents accidentally grabbing ladders when landing from jumps
    /// </remarks>
    private float ladderInteractionDelay = 0.1f;

    /// <summary>
    /// Time when the idle state was entered
    /// </summary>
    /// <remarks>
    /// Used to track the delay before allowing ladder interaction
    /// </remarks>
    private float timeEnteredState;

    /// <summary>
    /// Initializes a new instance of the PlayerIdleState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerIdleState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Performs state-specific checks
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();
    }

    /// <summary>
    /// Called when entering the idle state
    /// </summary>
    /// <remarks>
    /// Ensures the player has no horizontal velocity when entering idle state
    /// </remarks>
    public override void Enter()
    {
        base.Enter();
        Movement?.SetVelocityX(0f);
        timeEnteredState = Time.time;
    }

    /// <summary>
    /// Called when exiting the idle state
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Maintain zero horizontal velocity
    /// 2. Check for state transitions based on input:
    ///    - Horizontal input triggers movement state
    ///    - Vertical input near ladder triggers climb state
    ///    - Downward input without ladder triggers crouch state
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);

        if (!isExitingState)
        {
            if (xInput != 0)
            {
                stateMachine.ChangeState(player.MoveState);
            }
            else if (yInput != 0 && Time.time >= timeEnteredState + ladderInteractionDelay)
            {
                if (CollisionSenses.Ladder && ((yInput == -1 && CollisionSenses.LadderBottom) || (yInput == 1 && CollisionSenses.LadderTop)))
                {
                    if (player.LadderClimbState.CanEnterLadder())
                    {
                        player.PrepareClimb.SetClimbingDirection(yInput);
                        stateMachine.ChangeState(player.PrepareClimb);
                    }
                }
                else if (yInput == -1)
                    stateMachine.ChangeState(player.CrouchIdleState);
            }
        }
    }

    /// <summary>
    /// Updates physics-based components
    /// </summary>
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}