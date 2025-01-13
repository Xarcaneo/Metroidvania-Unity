using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State that handles the player's roll ability.
/// This state manages the rolling animation, movement, and invulnerability frames.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing roll animation and movement
/// - Adjusting collider height during roll
/// - Handling invulnerability during roll
/// - Managing roll cooldown
/// 
/// The state can transition to:
/// - IdleState: When roll duration completes
/// </remarks>
public class PlayerRollState : PlayerAbilityState
{
    #region State Variables
    /// <summary>
    /// Time when the last roll was performed
    /// </summary>
    private float lastRollTime;

    /// <summary>
    /// Whether the roll movement has started
    /// </summary>
    private bool startRoll = false;

    /// <summary>
    /// Whether the player is currently on a slope
    /// </summary>
    private bool isOnSlope;
    #endregion

    #region Core Components
    /// <summary>
    /// Reference to the CollisionSenses component, lazily loaded
    /// </summary>
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    /// <summary>
    /// Reference to the DamageReceiver component, lazily loaded
    /// </summary>
    protected DamageReceiver DamageReceiver { get => damageReceiver ?? core.GetCoreComponent(ref damageReceiver); }
    private DamageReceiver damageReceiver;

    /// <summary>
    /// Reference to the KnockbackReceiver component, lazily loaded
    /// </summary>
    protected KnockbackReceiver KnockbackReceiver { get => knockbackReceiver ?? core.GetCoreComponent(ref knockbackReceiver); }
    private KnockbackReceiver knockbackReceiver;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerRollState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerRollState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Called when entering the roll state
    /// </summary>
    /// <remarks>
    /// Sets up roll state by:
    /// 1. Adjusting collider height
    /// 2. Enabling invulnerability
    /// 3. Starting roll timer
    /// </remarks>
    public override void Enter()
    {
        base.Enter();

        // Adjust collider for rolling
        player.SetColliderHeight(playerData.crouchColliderHeight);

        player.RigidBody2D.sharedMaterial =  playerData.noFriction;

        // Enable invulnerability
        DamageReceiver.isDamagable = false;
        KnockbackReceiver.isKnockable = false;

        // Start roll timer
        startTime = Time.time;
    }

    /// <summary>
    /// Called when exiting the roll state
    /// </summary>
    /// <remarks>
    /// Cleans up roll state by:
    /// 1. Restoring collider height
    /// 2. Disabling invulnerability
    /// 3. Resetting roll flags
    /// </remarks>
    public override void Exit()
    {
        base.Exit();

        // Restore normal collider
        player.SetColliderHeight(playerData.standColliderHeight);

        // Ensure clean velocity on slope when exiting
        if (isOnSlope)
        {
            Movement?.SetVelocityXOnSlope(0f);
            Movement?.SetVelocityY(0f);
            player.RigidBody2D.sharedMaterial = playerData.fullFriction;
        }

        // Disable invulnerability
        DamageReceiver.isDamagable = true;
        KnockbackReceiver.isKnockable = true;
        startRoll = false;
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isOnSlope = CollisionSenses?.SlopeCheck() ?? false;
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Check roll duration
    /// 2. Apply roll movement
    /// 3. Transition to idle when complete
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        float rollTimeElapsed = Time.time - startTime;

        // Handle slope physics
        if (isOnSlope)
        {
            player.RigidBody2D.sharedMaterial = playerData.fullFriction;
            Movement?.SetVelocityY(0f);
        }
        else
        {
            player.RigidBody2D.sharedMaterial = playerData.noFriction;
        }

        // Check if roll duration is complete
        if (rollTimeElapsed >= playerData.rollDuration)
        {
            // Ensure smooth transition when ending roll on slope
            if (isOnSlope)
            {
                Movement?.SetVelocityXOnSlope(0f);
                Movement?.SetVelocityY(0f);
            }
            stateMachine.ChangeState(player.IdleState);
            return;
        }

        // Apply roll movement
        if (startRoll)
        {
            float rollVelocity = playerData.rollSpeed * Movement.FacingDirection;
            if (isOnSlope)
            {
                Movement?.SetVelocityXOnSlope(rollVelocity);
            }
            else
            {
                Movement?.SetVelocityX(rollVelocity);
            }
        }
        else
        {
            if (isOnSlope)
            {
                Movement?.SetVelocityXOnSlope(0f);
            }
            else
            {
                Movement?.SetVelocityX(0f);
            }
        }
    }

    /// <summary>
    /// Checks if the player can perform a roll
    /// </summary>
    /// <returns>True if enough time has passed since last roll, false otherwise</returns>
    /// <remarks>
    /// Verifies that the roll cooldown period has elapsed
    /// </remarks>
    public bool CheckIfCanRoll()
    {
        return Time.time >= lastRollTime + playerData.rollCooldown;
    }

    /// <summary>
    /// Triggered by animation events to start roll movement
    /// </summary>
    /// <remarks>
    /// Sets the startRoll flag to begin horizontal movement
    /// </remarks>
    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();
        startRoll = true;
    }
}
