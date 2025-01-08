using UnityEngine;

/// <summary>
/// State that handles the player's hurt behavior.
/// This state is activated when the player takes damage.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing damage and knockback response
/// - Handling invulnerability frames
/// - Transitioning to recovery state when appropriate
/// 
/// The state can transition to:
/// - RecoverState: When animation finishes and player is grounded
/// </remarks>
public class PlayerHurtState : PlayerState
{
    #region Check Variables
    /// <summary>
    /// Flag indicating if player is touching ground
    /// </summary>
    private bool isGrounded;
    #endregion

    #region Core Components
    /// <summary>
    /// Reference to the Movement component, lazily loaded
    /// </summary>
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    /// <summary>
    /// Reference to the CollisionSenses component, lazily loaded
    /// </summary>
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    /// <summary>
    /// Reference to the KnockbackReceiver component, lazily loaded
    /// </summary>
    private KnockbackReceiver KnockbackReceiver { get => knockbackReceiver ?? core.GetCoreComponent(ref knockbackReceiver); }
    private KnockbackReceiver knockbackReceiver;

    /// <summary>
    /// Reference to the DamageReceiver component, lazily loaded
    /// </summary>
    private DamageReceiver DamageReceiver { get => damageReceiver ?? core.GetCoreComponent(ref damageReceiver); }
    private DamageReceiver damageReceiver;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerHurtState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerHurtState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Performs state-specific checks
    /// </summary>
    /// <remarks>
    /// Updates ground contact status
    /// </remarks>
    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isGrounded = CollisionSenses.Ground;
        }
    }

    /// <summary>
    /// Called when entering the hurt state
    /// </summary>
    /// <remarks>
    /// Sets up damage and knockback event handlers and stops horizontal movement
    /// </remarks>
    public override void Enter()
    {
        base.Enter();

        DamageReceiver.OnDamage += OnDamageReceived;
        KnockbackReceiver.OnKnockback += OnKnockbackReceived;

        Movement?.SetVelocityX(0f);
    }

    /// <summary>
    /// Called when exiting the hurt state
    /// </summary>
    /// <remarks>
    /// Cleans up event handlers and resets damage and knockback flags
    /// </remarks>
    public override void Exit()
    {
        base.Exit();

        DamageReceiver.OnDamage -= OnDamageReceived;
        KnockbackReceiver.OnKnockback -= OnKnockbackReceived;

        KnockbackReceiver.isKnockable = true;
        DamageReceiver.isDamagable = true;
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Maintain zero horizontal velocity
    /// 2. Check for transition to recovery state when:
    ///    - Animation has finished
    ///    - Player is grounded
    ///    - Knockback is no longer active
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);

        if (isAnimationFinished && !isExitingState && !KnockbackReceiver.isKnockbackActive)
        {
            if (isGrounded && Movement?.CurrentVelocity.y < 0.01f)
            {
                stateMachine.ChangeState(player.RecoverState);
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

    #region Event Handlers
    /// <summary>
    /// Handles damage received event
    /// </summary>
    /// <param name="damage">Amount of damage received</param>
    /// <remarks>
    /// Disables further damage while in hurt state
    /// </remarks>
    private void OnDamageReceived(float damage) => DamageReceiver.isDamagable = false;

    /// <summary>
    /// Handles knockback received event
    /// </summary>
    /// <remarks>
    /// Disables further knockback while in hurt state
    /// </remarks>
    private void OnKnockbackReceived() => KnockbackReceiver.isKnockable = false;
    #endregion
}
