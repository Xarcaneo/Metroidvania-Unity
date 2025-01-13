using UnityEngine;

/// <summary>
/// State that handles the player's basic attack behavior.
/// Inherits from PlayerAbilityState to maintain ability-based functionality.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing the player's basic attack animations
/// - Dealing damage to enemies
/// - Handling block input during attacks
/// - Managing attack-specific physics (like friction on slopes)
/// 
/// The state automatically transitions to:
/// - PrepareBlockState: When block input is detected during attack
/// - IdleState: When the attack animation is finished
/// </remarks>
public class PlayerAttackState : PlayerAbilityState
{
    #region State Variables
    /// <summary>
    /// Flag indicating if the block input has been detected
    /// </summary>
    /// <remarks>
    /// Used to determine if the player wants to transition into blocking
    /// during the attack animation.
    /// </remarks>
    private bool blockInput;
    #endregion

    #region Core Components
    /// <summary>
    /// Reference to the DamageHitBox component, lazily loaded
    /// </summary>
    /// <remarks>
    /// Handles collision detection and damage application to enemies
    /// during the attack animation.
    /// </remarks>
    private DamageHitBox DamageHitBox { get => damageHitBox ?? core.GetCoreComponent(ref damageHitBox); }
    private DamageHitBox damageHitBox;

    /// <summary>
    /// Reference to the Stats component, lazily loaded
    /// </summary>
    /// <remarks>
    /// Provides access to player's attack stats for damage calculation.
    /// </remarks>
    protected Stats Stats { get => stats ?? core.GetCoreComponent(ref stats); }
    private Stats stats;
    #endregion

    /// <summary>
    /// Data structure containing damage information for the attack
    /// </summary>
    /// <remarks>
    /// Stores information about:
    /// - Damage source (the player)
    /// - Damage amount (from player's attack stat)
    /// - Any additional effects
    /// </remarks>
    private IDamageable.DamageData m_damageData;

    /// <summary>
    /// Initializes a new instance of the PlayerAttackState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerAttackState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Called when entering the attack state
    /// </summary>
    /// <remarks>
    /// Sets up the attack by:
    /// 1. Calling base class Enter method
    /// 2. Setting up damage data with player's attack stat
    /// 3. Consuming the attack input to prevent immediate re-trigger
    /// </remarks>
    public override void Enter()
    {
        base.Enter();

        // Initialize damage data with player's attack stat
        m_damageData.SetData(player, Stats.GetAttack());
        player.InputHandler.UseAttackInput();
    }

    /// <summary>
    /// Called when exiting the attack state
    /// </summary>
    /// <remarks>
    /// Cleans up the attack state by:
    /// 1. Calling base class Exit method
    /// 2. Stopping any horizontal movement
    /// </remarks>
    public override void Exit()
    {
        base.Exit();

        // Stop horizontal movement when exiting
        Movement?.SetVelocityX(0f);
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Call base class LogicUpdate
    /// 2. Ensure player remains stationary during attack
    /// 3. Check block input
    /// 4. Handle slope physics
    /// 5. Check for state transition conditions
    /// 
    /// The state will transition to:
    /// - PrepareBlockState: When block input is detected
    /// - IdleState: When the attack animation is finished
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Ensure player remains stationary during attack
        if (!isOnSlope)
        {
            Movement?.SetVelocityX(0f);
        }
        else
        {
            Movement?.SetVelocityXOnSlope(0f);
            Movement?.SetVelocityY(0f);
        }

        // Check for block input
        blockInput = player.InputHandler.BlockInput;

        // Handle slope physics during attack
        if (isOnSlope)
        {
            // Use full friction on slopes to prevent sliding
            player.RigidBody2D.sharedMaterial = playerData.fullFriction;
        }
        else
        {
            // Use no friction on flat ground
            player.RigidBody2D.sharedMaterial = playerData.noFriction;
        }

        // Handle state transitions
        if (blockInput)
        {
            stateMachine.ChangeState(player.PrepareBlockState);
        }
        else if (!isExitingState && isAnimationFinished)
        {
            stateMachine.ChangeState(player.IdleState);
            isAbilityDone = true;       
        }
    }

    /// <summary>
    /// Called by animation events during specific attack frames
    /// </summary>
    /// <remarks>
    /// Handles the actual damage application during the attack:
    /// 1. Calls base class AnimationActionTrigger
    /// 2. Applies melee damage to intersecting enemies
    /// 3. Applies knockback effect in player's facing direction
    /// </remarks>
    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        // Apply damage and knockback to enemies in weapon hitbox
        DamageHitBox?.MeleeAttack(m_damageData);
        DamageHitBox?.Knockback(m_damageData, Movement.FacingDirection);
    }
}
