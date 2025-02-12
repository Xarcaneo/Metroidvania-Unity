using UnityEngine;

/// <summary>
/// Base state for all spell casting related states.
/// This state provides common functionality for spell casting mechanics.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing basic spell casting mechanics
/// - Handling spell casting animations
/// - Managing spell casting resources (e.g., mana)
/// - Providing common transition logic for spell substates
/// 
/// The state can transition to:
/// - IdleState: When spell casting is interrupted or completed
/// - Various spell substates: Based on specific spell requirements
/// </remarks>
public class PlayerSpellCastState : PlayerState
{
    #region State Variables
    /// <summary>
    /// Flag indicating if the current spell cast has finished
    /// </summary>
    protected bool isSpellComplete;

    /// <summary>
    /// Flag indicating if the player is grounded
    /// </summary>
    protected bool isGrounded;

    /// <summary>
    /// Flag indicating if the player is touching a wall
    /// </summary>
    protected bool isTouchingWall;

    /// <summary>
    /// Flag indicating if the player is at a ledge
    /// </summary>
    protected bool isTouchingLedge;

    /// <summary>
    /// The currently selected spell from the hotbar
    /// </summary>
    protected Spell currentSpell;
    #endregion

    #region Core Components
    /// <summary>
    /// Reference to the CollisionSenses component, lazily loaded
    /// </summary>
    protected CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    /// <summary>
    /// Reference to the Movement component, lazily loaded
    /// </summary>
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    /// <summary>
    /// Reference to the PlayerCoreMagic component, lazily loaded
    /// </summary>
    /// <remarks>
    /// Provides access to spell casting functionality and cast type determination.
    /// Loaded on first access using core.GetCoreComponent().
    /// </remarks>
    protected PlayerCoreMagic PlayerMagic { get => playerMagic ?? core.GetCoreComponent(ref playerMagic); }
    private PlayerCoreMagic playerMagic;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerSpellCastState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerSpellCastState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Called when entering the spell cast state
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        isSpellComplete = false;
    }

    /// <summary>
    /// Called when exiting the spell cast state
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// Performs state-specific checks
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();
        
        if (CollisionSenses)
        {
            isGrounded = CollisionSenses.Ground;
            isTouchingWall = CollisionSenses.WallFront;
            isTouchingLedge = CollisionSenses.LedgeHorizontal;
        }
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isSpellComplete)
        {
            HandleStateTransition();
        }
    }

    /// <summary>
    /// Handles state transitions after spell completion
    /// </summary>
    /// <remarks>
    /// Determines the appropriate state to transition to based on current conditions:
    /// 1. IdleState: When grounded and not moving vertically
    /// 2. WallSlideState: When touching a wall
    /// 3. LedgeClimbState: When at a wall ledge
    /// 4. InAirState: When in the air and no other conditions are met
    /// 
    /// The order of these checks is important as it determines transition priority.
    /// </remarks>
    protected virtual void HandleStateTransition()
    {
        if (isGrounded && Movement.CurrentVelocity.y < 0.01f)
        {
            stateMachine.ChangeState(player.IdleState);
        }
        else if (isTouchingWall && !isTouchingLedge)
        {
            stateMachine.ChangeState(player.WallSlideState);
        }
        else if (isTouchingWall && isTouchingLedge)
        {
            stateMachine.ChangeState(player.LedgeClimbState);
        }
        else
        {
            stateMachine.ChangeState(player.InAirState);
        }
    }

    /// <summary>
    /// Initializes the spell casting process based on the hotbar number
    /// </summary>
    /// <param name="hotbarNumber">The index of the spell in the hotbar (0-based)</param>
    /// <returns>True if a spell was successfully initialized, false otherwise</returns>
    protected virtual void InitializeSpellCast(int hotbarNumber)
    {

    }
}
