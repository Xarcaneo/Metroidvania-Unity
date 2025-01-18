using PixelCrushers.DialogueSystem;
using UnityEngine;

/// <summary>
/// State that handles the player's basic attack behavior.
/// Inherits from PlayerAbilityState to maintain ability-based functionality.
/// </summary>
public class PlayerAttackState : PlayerAbilityState
{
    #region Constants

    /// <summary>
    /// Lua variable name for the player's unlocked attack combo count.
    /// </summary>
    private const string AttackComboUnlockedVar = "Player.AttackComboUnlocked";

    #endregion

    #region State Variables

    /// <summary>
    /// Flag indicating if the block input has been detected
    /// </summary>
    private bool blockInput;

    /// <summary>
    /// Tracks the player's current attack combo index.
    /// Incremented each time the player successfully chains an attack.
    /// </summary>
    private int currentCombo;

    /// <summary>
    /// Keeps track of the last time (via Time.time) the player exited the attack state.
    /// Used to determine if the combo should reset.
    /// </summary>
    private static float lastAttackExitTime = -999f;

    #endregion

    #region Core Components

    private DamageHitBox DamageHitBox => damageHitBox ?? core.GetCoreComponent(ref damageHitBox);
    private DamageHitBox damageHitBox;

    protected Stats Stats => stats ?? core.GetCoreComponent(ref stats);
    private Stats stats;

    #endregion

    /// <summary>
    /// Data structure containing damage information for the attack
    /// </summary>
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
        currentCombo = 0;
    }

    /// <summary>
    /// Called when entering the attack state
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        // If too much time has passed since the last attack exit, reset the combo.
        if (Time.time - lastAttackExitTime > playerData.breakComboTime)
        {
            currentCombo = 0;
        }

        // Initialize damage data with player's attack stat
        m_damageData.SetData(player, Stats.GetAttack());
        player.InputHandler.UseAttackInput();

        // Check how many combos are unlocked
        int maxCombo = DialogueLua.GetVariable(AttackComboUnlockedVar).AsInt;

        // Assign current combo index to animator
        player.Anim.SetInteger("attackCombo", currentCombo);

        // If we've reached or exceeded the max combo, reset
        if (currentCombo >= maxCombo)
            currentCombo = 0;
        else
            currentCombo++;
    }

    /// <summary>
    /// Called when exiting the attack state
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        // Stop horizontal movement when exiting
        Movement?.SetVelocityX(0f);

        // Record the time we exited this state
        lastAttackExitTime = Time.time;
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Ensure player remains stationary during attack
        Movement?.SetVelocityX(0f);

        // Check for block input
        blockInput = player.InputHandler.BlockInput;

        // Handle slope friction during attack
        if (isOnSlope)
        {
            player.RigidBody2D.sharedMaterial = playerData.fullFriction;
        }
        else
        {
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
    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        // Apply damage and knockback to enemies in the weapon hitbox
        DamageHitBox?.MeleeAttack(m_damageData);
        DamageHitBox?.Knockback(m_damageData, Movement.FacingDirection);
    }
}
