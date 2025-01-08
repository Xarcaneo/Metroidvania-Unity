using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

/// <summary>
/// State that handles the player's jump ability.
/// This state manages both initial jumps and mid-air jumps.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing jump velocity application
/// - Tracking available jumps
/// - Playing jump sound effects
/// - Transitioning to in-air state
/// 
/// The state transitions automatically to:
/// - InAirState: Immediately after applying jump velocity
/// </remarks>
public class PlayerJumpState : PlayerAbilityState
{
    #region State Variables
    /// <summary>
    /// Flag indicating if jumping is currently allowed
    /// </summary>
    public bool canJump = true;

    /// <summary>
    /// Number of jumps remaining before touching ground
    /// </summary>
    private int amountOfJumpsLeft;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerJumpState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerJumpState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
        amountOfJumpsLeft = playerData.amountOfJumps;
    }

    /// <summary>
    /// Called when entering the jump state
    /// </summary>
    /// <remarks>
    /// Performs the jump by:
    /// 1. Consuming the jump input
    /// 2. Applying jump velocity
    /// 3. Decrementing available jumps
    /// 4. Playing jump sound effect
    /// 5. Marking ability as complete
    /// </remarks>
    public override void Enter()
    {
        base.Enter();
        player.InputHandler.UseJumpInput();
        Movement?.SetVelocityY(playerData.jumpVelocity);
        isAbilityDone = true;
        amountOfJumpsLeft--;
        player.InAirState.SetIsJumping();

        // Play jump sound effect
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/ActorsEvents/PlayerEvents/PlayerJump", this.player.transform.position);
    }

    /// <summary>
    /// Checks if the player can perform a jump
    /// </summary>
    /// <returns>True if player has remaining jumps and jumping is allowed, false otherwise</returns>
    /// <remarks>
    /// Jump is allowed when:
    /// 1. Player has jumps remaining (amountOfJumpsLeft > 0)
    /// 2. Jumping is not disabled (canJump is true)
    /// </remarks>
    public bool CanJump()
    {
        if (amountOfJumpsLeft > 0 && canJump)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Resets the number of available jumps to maximum
    /// </summary>
    /// <remarks>
    /// Called when player touches ground or other jump-resetting conditions
    /// </remarks>
    public void ResetAmountOfJumpsLeft() => amountOfJumpsLeft = playerData.amountOfJumps;

    /// <summary>
    /// Decreases the number of available jumps by one
    /// </summary>
    /// <remarks>
    /// Called when coyote time expires without jumping
    /// </remarks>
    public void DecreaseAmountOfJumpsLeft() => amountOfJumpsLeft--;
}