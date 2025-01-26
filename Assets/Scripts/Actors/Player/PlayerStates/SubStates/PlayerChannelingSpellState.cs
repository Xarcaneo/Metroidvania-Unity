using UnityEngine;

/// <summary>
/// State for channeling spells that require holding the cast button.
/// </summary>
public class PlayerChannelingSpellState : PlayerSpellCastState
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
    #endregion

    /// <summary>
    /// Initializes a new instance of the PlayerChannelingSpellState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerChannelingSpellState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Called when entering the channeling state
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        
        stateTimer = 0f;
        channelDuration = PlayerMagic.currentSpell.channelingTime;
    }

    /// <summary>
    /// Called when exiting the channeling state
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        stateTimer = 0f;
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            stateTimer += Time.deltaTime;

            // Exit if we release the button or exceed channel duration
            if (!player.InputHandler.SpellCastInput || stateTimer >= channelDuration)
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }
}
