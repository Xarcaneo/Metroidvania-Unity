using UnityEngine;

/// <summary>
/// Superstate that handles player behavior when touching a wall.
/// This state manages wall sliding, wall jumping, and wall grip mechanics.
/// </summary>
/// <remarks>
/// This state serves as a base class for wall-related states like WallSlide and WallGrip.
/// It handles the basic wall detection and state transitions while providing common functionality
/// for derived states. The state automatically transitions to:
/// - InAirState: When player is no longer touching the wall or facing away from it
/// - GripWallState: When player is touching a grippable wall and presses attack
/// - IdleState: When player touches the ground
/// </remarks>
public class PlayerTouchingWallState : PlayerState
{
    #region State Variables
    /// <summary>
    /// Flag indicating if the player is touching the ground
    /// </summary>
    /// <remarks>
    /// Updated every frame in DoChecks(). Used to transition to IdleState when true.
    /// </remarks>
    protected bool isGrounded;

    /// <summary>
    /// Flag indicating if the player is touching a wall
    /// </summary>
    /// <remarks>
    /// Updated every frame in DoChecks(). Used to determine if player should stay in this state.
    /// When false, transitions to InAirState.
    /// </remarks>
    protected bool isTouchingWall;

    /// <summary>
    /// Flag indicating if the player is touching a wall that can be gripped
    /// </summary>
    /// <remarks>
    /// Updated every frame in DoChecks(). Used in combination with attackInput to transition
    /// to GripWallState. Not all walls are grippable - this depends on the wall's layer and properties.
    /// </remarks>
    protected bool isTouchingGripWall;

    /// <summary>
    /// Normalized horizontal input (-1, 0, 1)
    /// </summary>
    /// <remarks>
    /// Updated every frame in LogicUpdate(). Used to determine if player is trying to move
    /// away from the wall. When input direction doesn't match wall direction, transitions to InAirState.
    /// </remarks>
    protected int xInput;

    /// <summary>
    /// Normalized vertical input (-1, 0, 1)
    /// </summary>
    /// <remarks>
    /// Updated every frame in LogicUpdate(). Can be used by derived states for wall climb
    /// or slide speed modification.
    /// </remarks>
    protected int yInput;

    /// <summary>
    /// Flag indicating if attack button is pressed
    /// </summary>
    /// <remarks>
    /// Updated every frame in LogicUpdate(). Used in combination with isTouchingGripWall
    /// to transition to GripWallState when player attempts to grip a grippable wall.
    /// </remarks>
    protected bool attackInput;
    #endregion

    #region Core Components
    /// <summary>
    /// Reference to the Movement component, lazily loaded
    /// </summary>
    /// <remarks>
    /// Provides access to movement-related functionality like velocity control and facing direction.
    /// Loaded on first access using core.GetCoreComponent().
    /// </remarks>
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    /// <summary>
    /// Reference to the CollisionSenses component, lazily loaded
    /// </summary>
    /// <remarks>
    /// Provides access to collision detection functionality for ground, walls, and other surfaces.
    /// Loaded on first access using core.GetCoreComponent().
    /// </remarks>
    protected CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the PlayerTouchingWallState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerTouchingWallState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }
    #endregion

    #region State Methods
    /// <summary>
    /// Performs state checks for collision detection
    /// </summary>
    /// <remarks>
    /// Called every frame to update collision flags. These flags are used to determine
    /// state transitions and behavior modifications.
    /// </remarks>
    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isGrounded = CollisionSenses.Ground;
            isTouchingWall = CollisionSenses.WallFront;
            isTouchingGripWall = collisionSenses.GripWall;
        }
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Update input values from the input handler
    /// 2. Check state transition conditions
    /// 3. Transition to appropriate states based on conditions
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Update input values
        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        attackInput = player.InputHandler.AttackInput;

        // Check state transitions
        if (!isTouchingWall || xInput != Movement?.FacingDirection)
        {
            // Transition to InAirState if not touching wall or facing away from it
            stateMachine.ChangeState(player.InAirState);
        }
        else if(isTouchingGripWall && attackInput)
        {
            // Transition to GripWallState if touching grippable wall and attack pressed
            stateMachine.ChangeState(player.GripWallState);
        }
        else if (isGrounded)
        {
            // Transition to IdleState if touching ground
            stateMachine.ChangeState(player.IdleState);
        }
    }

    /// <summary>
    /// Called when an animation finish event is triggered
    /// </summary>
    /// <remarks>
    /// Can be used by derived states to handle animation-specific logic
    /// when their animations complete.
    /// </remarks>
    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    /// <summary>
    /// Called when an animation trigger event occurs
    /// </summary>
    /// <remarks>
    /// Can be used by derived states to handle animation-specific events
    /// during their animations.
    /// </remarks>
    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
    }

    /// <summary>
    /// Called when entering the state
    /// </summary>
    /// <remarks>
    /// Can be used by derived states to set up their initial conditions
    /// when transitioning into the state.
    /// </remarks>
    public override void Enter()
    {
        base.Enter();
    }

    /// <summary>
    /// Called during the physics update
    /// </summary>
    /// <remarks>
    /// Can be used by derived states to apply physics-based modifications
    /// like changing velocity or applying forces.
    /// </remarks>
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    #endregion
}