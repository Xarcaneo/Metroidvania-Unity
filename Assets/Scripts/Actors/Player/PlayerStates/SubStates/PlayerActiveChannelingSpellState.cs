using UnityEngine;

/// <summary>
/// State for actively channeling spells that require holding the cast button.
/// This state handles the actual channeling logic after the start animation is complete.
/// </summary>
public class PlayerActiveChannelingSpellState : PlayerSpellCastState
{
    #region State Variables
    /// <summary>
    /// Timer to track channeling duration
    /// </summary>
    private float stateTimer;

    /// <summary>
    /// Maximum duration the spell can be channeled
    /// </summary>
    private float channelDuration;

    /// <summary>
    /// Initial hotbar slot number
    /// </summary>
    private int initialHotbarSlot;
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerActiveChannelingSpellState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerActiveChannelingSpellState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Called when entering the active channeling state
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        
        stateTimer = 0f;
        channelDuration = PlayerMagic.currentSpell.channelingTime;
        initialHotbarSlot = player.InputHandler.UseSpellHotbarNumber;
        
        // Start the channeling bar animation
        PlayerMagic.StartChannelingBar(channelDuration);
    }

    /// <summary>
    /// Called when exiting the channeling state
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        player.InputHandler.UseSpellCastInput();
        stateTimer = 0f;

        // Stop the channeling bar animation
        PlayerMagic.StopChannelingBar();
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Ensure player remains stationary during block
        Movement?.SetVelocityX(0f);

        if (!isExitingState)
        {
            // Check if player changed hotbar slot
            if (player.InputHandler.UseSpellHotbarNumber != initialHotbarSlot)
            {
                PlayerMagic.StopChannelingBar();
                stateMachine.ChangeState(player.IdleState);
                return;
            }

            stateTimer += Time.deltaTime;

            // Check if channeling is complete
            if (stateTimer >= channelDuration)
            {
                // Cast the spell and exit
                PlayerMagic.currentSpell.Cast(player, null);
                stateMachine.ChangeState(player.IdleState);
            }
            // Check if player released the button early
            else if (!player.InputHandler.SpellCastInput)
            {
                // Just exit without casting
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }
}
