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
    #endregion

    #region Core Components
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

        // Disable invulnerability
        DamageReceiver.isDamagable = true;
        KnockbackReceiver.isKnockable = true;
        startRoll = false;
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

        // Check if roll duration is complete
        if (rollTimeElapsed >= playerData.rollDuration)
        {
            stateMachine.ChangeState(player.IdleState);
            return;
        }

        // Apply roll movement
        if (startRoll)
            Movement?.SetVelocityX(playerData.rollSpeed * Movement.FacingDirection);
        else
            Movement?.SetVelocityX(0f);
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
