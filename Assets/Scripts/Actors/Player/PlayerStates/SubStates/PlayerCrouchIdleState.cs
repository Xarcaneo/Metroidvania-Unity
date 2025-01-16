using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State that handles the player's crouching idle behavior.
/// Inherits from PlayerGroundedState to maintain ground-based functionality.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing the player's crouching state
/// - Adjusting collider height for crouching
/// - Handling transitions between crouching and standing
/// - Maintaining zero horizontal velocity while crouching
/// 
/// The state automatically transitions to:
/// - IdleState: When vertical input is no longer down (-1)
/// </remarks>
public class PlayerCrouchIdleState : PlayerGroundedState
{
    /// <summary>
    /// Flag indicating if the player is currently in a crouching state
    /// </summary>
    /// <remarks>
    /// This flag is used by other states to check if the player is crouching,
    /// particularly important for preventing certain actions while crouched
    /// (e.g., jumping, attacking).
    /// </remarks>
    public bool isCrouching = false;

    private bool m_hasTriggeredDropThrough = false;

    /// <summary>
    /// Initializes a new instance of the PlayerCrouchIdleState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerCrouchIdleState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Called when entering the crouch idle state
    /// </summary>
    /// <remarks>
    /// Sets up the crouching state by:
    /// 1. Calling base class Enter method
    /// 2. Setting isCrouching flag to true
    /// 3. Adjusting player's collider height to crouching height
    /// </remarks>
    public override void Enter()
    {
        base.Enter();

        m_hasTriggeredDropThrough = false;
        isCrouching = true;
        player.SetColliderHeight(playerData.rollColliderHeight);
    }

    /// <summary>
    /// Called when exiting the crouch idle state
    /// </summary>
    /// <remarks>
    /// Cleans up the crouching state by:
    /// 1. Calling base class Exit method
    /// 2. Setting isCrouching flag to false
    /// 3. Restoring player's collider height to standing height
    /// </remarks>
    public override void Exit()
    {
        base.Exit();

        isCrouching = false;
        player.SetColliderHeight(playerData.standColliderHeight);
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Call base class LogicUpdate
    /// 2. Set horizontal velocity to zero (player can't move while crouching)
    /// 3. Check for state transition conditions
    /// 
    /// The state will transition to IdleState when:
    /// - The player is not currently exiting the state AND
    /// - The vertical input is no longer down (-1)
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Ensure player can't move horizontally while crouching
        Movement?.SetVelocityX(0f);

        if (!isExitingState)
        {
            // Check for platform drop-through
            if (jumpInput && !m_hasTriggeredDropThrough)
            {
                HandlePlatformDropThrough();
                m_hasTriggeredDropThrough = true;
            }
            // Transition to idle if player releases crouch
            else if (yInput != -1)
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }

    /// <summary>
    /// Handles the platform drop-through mechanic when the player is crouching and presses jump.
    /// Casts multiple raycasts across the player's width to detect all platform segments beneath,
    /// disables collision with these platforms, and applies a downward force to initiate falling.
    /// </summary>
    /// <remarks>
    /// The method uses the CollisionSenses component to:
    /// - Get the platform check position
    /// - Determine the width of the check based on ground check offset
    /// - Use the correct layer mask for platform detection
    /// 
    /// Multiple raycasts are used to ensure all connected platform segments are detected
    /// and disabled simultaneously, preventing the player from getting stuck on partial segments.
    /// </remarks>
    private void HandlePlatformDropThrough()
    {
        Vector2 rayStart = CollisionSenses.PlatformCheck.position;
        float rayDistance = 2f;
        float checkWidth = CollisionSenses.GroundCheckOffset * 2f;
        
        // Do multiple raycasts across the check width
        int numRays = 3;
        bool foundAnyPlatform = false;
        HashSet<IPlatform> platformsToDisable = new HashSet<IPlatform>();

        for (int i = 0; i < numRays; i++)
        {
            float xOffset = (i / (float)(numRays - 1) - 0.5f) * checkWidth;
            Vector2 rayOrigin = rayStart + new Vector2(xOffset, 0);
            
            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                Vector2.down,
                rayDistance,
                CollisionSenses.WhatIsPlatform
            );
            
            if (hit.collider != null)
            {
                var platform = hit.collider.GetComponent<IPlatform>();
                if (platform != null)
                {
                    platformsToDisable.Add(platform);
                    foundAnyPlatform = true;
                }
            }
        }

        // If we found any platforms, disable them all and apply downward force
        if (foundAnyPlatform)
        {
            foreach (var platform in platformsToDisable)
            {
                platform.DropThrough();
            }
            
            // Apply downward force to start falling
            Movement?.SetVelocityY(-5f);
        }
    }
}