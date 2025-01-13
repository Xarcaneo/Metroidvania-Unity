using UnityEngine;

/// <summary>
/// Superstate that handles player behavior when on the ground.
/// This state manages ground-based movement, actions, and transitions to other states.
/// </summary>
/// <remarks>
/// This state serves as a base class for ground-based states like Idle, Move, and Crouch.
/// It handles common ground functionality including:
/// - Movement and slope physics
/// - Action inputs (attack, block, roll)
/// - State transitions based on input and environment
/// - Jump initialization and conditions
/// 
/// The state automatically transitions to:
/// - AttackState: When attack button is pressed
/// - PrepareBlockState: When block button is pressed
/// - UseHotbarItem: When hotbar action button is pressed with non-empty slot
/// - InAirState: When no longer grounded (with coyote time)
/// - RollState: When action button is pressed and roll is available
/// - JumpState: When jump conditions are met
/// </remarks>
public class PlayerGroundedState : PlayerState
{
    #region Input Variables

    /// <summary>
    /// Normalized horizontal input (-1, 0, 1)
    /// </summary>
    /// <remarks>
    /// Updated every frame in LogicUpdate(). Used for movement direction and slope physics.
    /// </remarks>
    protected int xInput;

    /// <summary>
    /// Normalized vertical input (-1, 0, 1)
    /// </summary>
    /// <remarks>
    /// Updated every frame in LogicUpdate(). Used for ladder climbing and menu navigation.
    /// </remarks>
    protected int yInput;

    /// <summary>
    /// Flag indicating if hotbar item use button is pressed
    /// </summary>
    /// <remarks>
    /// Updated every frame in LogicUpdate(). Triggers transition to UseHotbarItem state
    /// when pressed with a non-empty hotbar slot.
    /// </remarks>
    protected bool useHotbarItemInput;

    /// <summary>
    /// Flag indicating if jump button is pressed
    /// </summary>
    /// <remarks>
    /// Updated every frame in LogicUpdate(). Triggers transition to JumpState when
    /// conditions are met (not crouching, minimal Y velocity).
    /// </remarks>
    private bool jumpInput;

    /// <summary>
    /// Flag indicating if attack button is pressed
    /// </summary>
    /// <remarks>
    /// Updated every frame in LogicUpdate(). Immediately triggers transition to
    /// AttackState when pressed.
    /// </remarks>
    private bool attackInput;

    /// <summary>
    /// Flag indicating if block button is pressed
    /// </summary>
    /// <remarks>
    /// Updated every frame in LogicUpdate(). Immediately triggers transition to
    /// PrepareBlockState when pressed.
    /// </remarks>
    private bool blockInput;

    /// <summary>
    /// Flag indicating if action (roll) button is pressed
    /// </summary>
    /// <remarks>
    /// Updated every frame in LogicUpdate(). Triggers transition to RollState when
    /// pressed and roll conditions are met.
    /// </remarks>
    private bool actionInput;

    #endregion

    #region State Variables

    /// <summary>
    /// Flag indicating if player is touching the ground
    /// </summary>
    /// <remarks>
    /// Updated every frame in DoChecks(). When false, triggers transition to InAirState
    /// with coyote time enabled.
    /// </remarks>
    private bool isGrounded;

    /// <summary>
    /// Flag indicating if player is touching a ladder
    /// </summary>
    /// <remarks>
    /// Updated every frame in DoChecks(). Used by derived states to handle
    /// ladder climbing transitions.
    /// </remarks>
    protected bool isTouchingLadder;

    /// <summary>
    /// Flag indicating if player is on a slope
    /// </summary>
    /// <remarks>
    /// Updated every frame in DoChecks(). Affects physics material and vertical
    /// velocity handling when true.
    /// </remarks>
    protected bool isOnSlope;

    /// <summary>
    /// Flag indicating if player is touching a wall
    /// </summary>
    /// <remarks>
    /// Updated every frame in DoChecks(). Used by derived states to prevent
    /// movement into walls and handle wall interactions.
    /// </remarks>
    protected bool isTouchingWall;

    #endregion

    #region Core Components

    /// <summary>
    /// Reference to the Movement component, lazily loaded
    /// </summary>
    /// <remarks>
    /// Provides access to movement-related functionality like velocity control
    /// and physics-based movement. Loaded on first access using core.GetCoreComponent().
    /// </remarks>
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    /// <summary>
    /// Reference to the CollisionSenses component, lazily loaded
    /// </summary>
    /// <remarks>
    /// Provides access to collision detection functionality for ground, walls,
    /// ladders, and slopes. Loaded on first access using core.GetCoreComponent().
    /// </remarks>
    protected CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the PlayerGroundedState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerGroundedState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName)
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    #endregion

    #region State Methods

    /// <summary>
    /// Performs state checks for collision detection
    /// </summary>
    /// <remarks>
    /// Called every frame to update collision flags. These flags determine:
    /// - Ground contact for state transitions
    /// - Ladder presence for climbing
    /// - Slope detection for physics adjustments
    /// - Wall contact for movement restrictions
    /// </remarks>
    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isGrounded = CollisionSenses.Ground;
            isTouchingLadder = collisionSenses.Ladder;
            isOnSlope = CollisionSenses.SlopeCheck();
            isTouchingWall = CollisionSenses.WallFront;
        }
    }

    /// <summary>
    /// Called when entering the state
    /// </summary>
    /// <remarks>
    /// Resets the number of available jumps when entering ground state.
    /// This allows the player to perform aerial moves after leaving the ground.
    /// </remarks>
    public override void Enter()
    {
        base.Enter();
        player.JumpState.ResetAmountOfJumpsLeft();
    }

    /// <summary>
    /// Called when exiting the state
    /// </summary>
    /// <remarks>
    /// Ensures proper physics material is set when leaving ground state.
    /// This prevents sticking to surfaces when transitioning to air states.
    /// </remarks>
    public override void Exit()
    {
        base.Exit();
        HandleSlopePhysics();
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Update input values from the input handler
    /// 2. Handle slope physics adjustments
    /// 3. Check and execute state transitions based on input and conditions
    /// 
    /// State transitions are checked in priority order:
    /// 1. Attack
    /// 2. Block
    /// 3. Use Hotbar Item
    /// 4. Fall (when not grounded)
    /// 5. Roll
    /// 6. Jump
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Get input states
        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        jumpInput = player.InputHandler.JumpInput;
        actionInput = player.InputHandler.ActionInput;
        attackInput = player.InputHandler.AttackInput;
        blockInput = player.InputHandler.BlockInput;
        useHotbarItemInput = player.InputHandler.HotbarActionInput;

        // Handle slope physics
        HandleSlopePhysics();

        // Check state transitions in priority order
        if (attackInput)
        {
            stateMachine.ChangeState(player.AttackState);
        }
        else if (blockInput)
        {
            stateMachine.ChangeState(player.PrepareBlockState);
        }
        else if (useHotbarItemInput && !Menu.GameMenu.Instance.gameHotbar.IsSlotEmpty())
        {
            stateMachine.ChangeState(player.UseHotbarItem);
        }
        else if (!isGrounded)
        {
            player.InAirState.StartCoyoteTime();
            stateMachine.ChangeState(player.InAirState);
        }
        else if (actionInput && player.RollState.CheckIfCanRoll())
        {
            stateMachine.ChangeState(player.RollState);
        }
        else if (jumpInput && player.JumpState.CanJump() && !player.CrouchIdleState.isCrouching)
        {
            float velocityThreshold = 0.01f;
            if (Mathf.Abs(Movement.CurrentVelocity.y) < velocityThreshold)
            {
                stateMachine.ChangeState(player.JumpState);
            }
        }
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

    #region Helper Methods

    /// <summary>
    /// Handles physics behavior when on slopes
    /// </summary>
    /// <remarks>
    /// Manages the physics material and vertical velocity when on slopes:
    /// - Uses full friction when stationary or crouching to prevent sliding
    /// - Uses no friction when moving to allow smooth slope traversal
    /// - Zeros out vertical velocity to prevent unwanted vertical movement
    /// </remarks>
    private void HandleSlopePhysics()
    {
        if (player.CrouchIdleState.isCrouching)
        {
            SetFriction(true);
        }
        else if (isOnSlope)
        {
            Movement?.SetVelocityY(0.0f);

            if (xInput == 0.0f)
            {
                SetFriction(true);
            }
            else
            {
                SetFriction(false);
            }
        }
        else
        {
            SetFriction(false);
        }
    }

    /// <summary>
    /// Sets the appropriate friction material for the player
    /// </summary>
    /// <param name="useFullFriction">If true, uses full friction. If false, uses no friction.</param>
    protected void SetFriction(bool useFullFriction)
    {
        player.RigidBody2D.sharedMaterial = useFullFriction ? playerData.fullFriction : playerData.noFriction;
    }

    /// <summary>
    /// Handles transition to jump state with velocity check
    /// </summary>
    /// <remarks>
    /// Ensures player can only jump when vertical velocity is minimal.
    /// This prevents unwanted double jumps or jumps during landing/falling.
    /// The velocity threshold can be adjusted to fine-tune jump responsiveness.
    /// </remarks>
    private void HandleJumpTransition()
    {
        float velocityThreshold = 0.01f;
        if (Mathf.Abs(Movement.CurrentVelocity.y) < velocityThreshold)
        {
            stateMachine.ChangeState(player.JumpState);
        }
    }

    #endregion
}
