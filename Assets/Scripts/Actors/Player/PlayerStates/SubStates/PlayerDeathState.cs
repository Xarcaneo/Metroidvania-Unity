using UnityEngine;

/// <summary>
/// State that handles the player's death sequence.
/// This state manages the death animation and triggers game-wide death events.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Playing the death animation
/// - Ensuring player remains stationary during death
/// - Checking for ground contact
/// - Triggering game-wide death events
/// 
/// The state triggers death events when:
/// - The death animation is complete AND
/// - The player has made contact with the ground
/// </remarks>
public class PlayerDeathState : PlayerState
{
    #region State Variables
    /// <summary>
    /// Flag indicating if the player is in contact with the ground
    /// </summary>
    private bool isGrounded;
    #endregion

    #region Core Components
    /// <summary>
    /// Reference to the Movement component, lazily loaded
    /// </summary>
    /// <remarks>
    /// Used to control player movement during death sequence,
    /// ensuring the player remains stationary.
    /// </remarks>
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    /// <summary>
    /// Reference to the CollisionSenses component, lazily loaded
    /// </summary>
    /// <remarks>
    /// Used to detect when the player makes contact with the ground
    /// during the death sequence.
    /// </remarks>
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    /// <summary>
    /// Reference to the PlayerDeath component, lazily loaded
    /// </summary>
    /// <remarks>
    /// Handles the actual death functionality and game state changes
    /// when the death sequence is complete.
    /// </remarks>
    private PlayerDeath PlayerDeath { get => playerDeath ?? core.GetCoreComponent(ref playerDeath); }
    private PlayerDeath playerDeath;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerDeathState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerDeathState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Performs state-specific checks
    /// </summary>
    /// <remarks>
    /// Updates ground contact status by checking if the player
    /// is touching the ground using CollisionSenses.
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
    /// Called when entering the death state
    /// </summary>
    /// <remarks>
    /// Sets up the death sequence by:
    /// 1. Calling base class Enter method
    /// 2. Ensuring player remains stationary
    /// </remarks>
    public override void Enter()
    {
        base.Enter();

        // Ensure player stops moving when death begins
        Movement?.SetVelocityX(0f);
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Call base class LogicUpdate
    /// 2. Ensure player remains stationary
    /// 3. Check if death sequence should complete
    /// 
    /// The death sequence completes when:
    /// - The death animation has finished AND
    /// - The player has made contact with the ground
    /// 
    /// Upon completion:
    /// 1. Triggers the global PlayerDied event
    /// 2. Calls the PlayerDeath component's Die method
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Ensure player remains stationary during death
        Movement?.SetVelocityX(0f);

        // Complete death sequence when animation is done and player is grounded
        if (isAnimationFinished && isGrounded)
        {
            GameEvents.Instance.PlayerDied();
            PlayerDeath.Die();
        }
    }
}
