using UnityEngine;

/// <summary>
/// Base class for all player states in the finite state machine.
/// Provides common functionality and data access for player-specific states.
/// </summary>
public class PlayerState : State
{
    #region Protected Fields
    /// <summary>
    /// Reference to the player component this state belongs to
    /// </summary>
    protected readonly Player player;

    /// <summary>
    /// Reference to the player's configuration data
    /// </summary>
    protected readonly PlayerData playerData;
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the PlayerState class
    /// </summary>
    /// <param name="player">The player component this state belongs to</param>
    /// <param name="stateMachine">The state machine managing this state</param>
    /// <param name="playerData">Configuration data for the player</param>
    /// <param name="animBoolName">Name of the animation boolean parameter</param>
    public PlayerState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, animBoolName)
    {
        this.player = player;
        this.playerData = playerData;
    }
    #endregion

    #region State Methods
    /// <summary>
    /// Called when entering this state. Notifies game events about the state change.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        GameEvents.Instance.PlayerStateChanged(this);
    }
    #endregion
}