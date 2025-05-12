using UnityEngine;

/// <summary>
/// State that handles the preparation phase before blocking an attack.
/// This state manages the transition between normal states and blocking state.
/// </summary>
/// <remarks>
/// This state is responsible for:
/// - Managing block preparation animation
/// - Handling block input and successful block detection
/// - Transitioning to appropriate states based on block success
/// 
/// The state can transition to:
/// - BlockState: When a successful block is performed
/// - IdleState: When preparation animation finishes without a successful block
/// </remarks>
public class PlayerPrepareBlockState : PlayerAbilityState
{
    #region Core Components
    /// <summary>
    /// Reference to the Block component, lazily loaded
    /// </summary>
    private Block Block { get => block ?? core.GetCoreComponent(ref block); }
    private Block block;

    /// <summary>
    /// Reference to the DamageReceiver component, lazily loaded
    /// </summary>
    private DamageReceiver DamageReceiver { get => damageReceiver ?? core.GetCoreComponent(ref damageReceiver); }
    private DamageReceiver damageReceiver;
    #endregion

    #region State Variables
    /// <summary>
    /// Indicates whether a block was successfully performed
    /// </summary>
    private bool successfulBlock = false;
    
    /// <summary>
    /// Indicates whether the blocked attack can be parried
    /// </summary>
    private bool canParryBlock = false;

    private float blockTimer;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerPrepareBlockState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerPrepareBlockState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Called when entering the prepare block state
    /// </summary>
    /// <remarks>
    /// Sets up block detection and consumes block input
    /// </remarks>
    public override void Enter()
    {
        base.Enter();

        // Subscribe to block detection event
        DamageReceiver.OnSuccessfulBlock += OnSuccesfullBlock;

        // Consume block input to prevent double triggering
        player.InputHandler.UseBlockInput();

        blockTimer = 0f;
    }

    /// <summary>
    /// Called when exiting the prepare block state
    /// </summary>
    /// <remarks>
    /// Cleans up block detection and resets block state
    /// </remarks>
    public override void Exit()
    {
        base.Exit();

        // Unsubscribe from block detection event
        DamageReceiver.OnSuccessfulBlock -= OnSuccesfullBlock;

        // Reset block state
        successfulBlock = false;
        Block.isBlocking = false;
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    /// <remarks>
    /// Called every frame to:
    /// 1. Stop horizontal movement during block preparation
    /// 2. Check for successful block
    /// 3. Transition to appropriate state based on block success
    /// </remarks>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Ensure player remains stationary during prepare block
        Movement?.SetVelocityX(0f);

        if (successfulBlock)
        {
            // Pass the canParryBlock flag to the BlockState
            player.BlockState.SetCanParry(canParryBlock);
            stateMachine.ChangeState(player.BlockState);
        }
        else if (isAnimationFinished)
        {
            blockTimer += Time.deltaTime;
            if (blockTimer >= playerData.BlockStateDuration)
            {
                stateMachine.ChangeState(player.IdleState);
                return;
            }
        }
    }

    /// <summary>
    /// Called when a successful block is performed
    /// </summary>
    /// <remarks>
    /// Sets the successful block flag to trigger state transition
    /// and checks if the blocked attack can be parried
    /// </remarks>
    private void OnSuccesfullBlock()
    {
        successfulBlock = true;
        
        // Check if the blocked attack can be parried
        canParryBlock = DamageReceiver.CanParryLastBlock;
    }

    /// <summary>
    /// Called by animation events to toggle block state
    /// </summary>
    /// <remarks>
    /// Toggles the blocking flag on the Block component
    /// </remarks>
    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();

        // Toggle block state
        if (!Block.isBlocking) Block.isBlocking = true;
        else Block.isBlocking = false;
    }
}
