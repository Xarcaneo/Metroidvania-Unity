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
            if (jumpInput)
            {
                // Use the platform check point from CollisionSenses
                Vector2 rayStart = CollisionSenses.PlatformCheck.position;
                float rayDistance = 2f; // Much longer distance to ensure we can reach the platform

                // Use the platform layer mask from CollisionSenses
                RaycastHit2D hit = Physics2D.Raycast(
                    rayStart,
                    Vector2.down,
                    rayDistance,
                    CollisionSenses.WhatIsPlatform
                );

                // Debug visualization that will be visible in game view
                Debug.Log($"Ray Start: {rayStart}, Ray End: {rayStart + Vector2.down * rayDistance}, Layer: {CollisionSenses.WhatIsPlatform.value}");
                Debug.Log($"Platform check: Hit={hit.collider?.name ?? "null"}, Distance={hit.distance}, Point={hit.point}");
                
                // Draw a vertical line showing the full raycast path
                Debug.DrawLine(rayStart, rayStart + Vector2.down * rayDistance, hit.collider != null ? Color.green : Color.red, 5f);
                
                // Draw crosses at both start and end points
                float crossSize = 0.2f;
                // Start point cross (yellow)
                Debug.DrawLine(
                    rayStart - Vector2.right * crossSize, 
                    rayStart + Vector2.right * crossSize, 
                    Color.yellow, 
                    5f
                );
                Debug.DrawLine(
                    rayStart - Vector2.up * crossSize, 
                    rayStart + Vector2.up * crossSize, 
                    Color.yellow, 
                    5f
                );
                
                // End point cross (blue)
                Vector2 rayEnd = rayStart + Vector2.down * rayDistance;
                Debug.DrawLine(
                    rayEnd - Vector2.right * crossSize, 
                    rayEnd + Vector2.right * crossSize, 
                    Color.blue, 
                    5f
                );
                Debug.DrawLine(
                    rayEnd - Vector2.up * crossSize, 
                    rayEnd + Vector2.up * crossSize, 
                    Color.blue, 
                    5f
                );

                if (hit.collider != null)
                {
                    var platform = hit.collider.GetComponent<IPlatform>();
                    Debug.Log($"Found platform component: {platform != null}");
                    if (platform != null)
                    {
                        Debug.Log($"Calling DropThrough on platform: {hit.collider.name}");
                        platform.DropThrough();
                    }
                    else
                    {
                        Debug.LogWarning($"Found collider {hit.collider.name} but it doesn't have IPlatform component!");
                    }
                }
            }
            // Transition to idle if player releases crouch
            else if (yInput != -1)
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }
}