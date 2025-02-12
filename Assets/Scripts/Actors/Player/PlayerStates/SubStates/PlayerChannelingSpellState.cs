using UnityEngine;

/// <summary>
/// State for playing the starting animation of channeling spells.
/// </summary>
public class PlayerChannelingSpellStartState : PlayerSpellCastState
{
    /// <summary>
    /// Initializes a new instance of the PlayerChannelingSpellStartState
    /// </summary>
    /// <param name="player">Reference to the Player component</param>
    /// <param name="stateMachine">Reference to the state machine managing player states</param>
    /// <param name="playerData">Reference to the player's data container</param>
    /// <param name="animBoolName">Name of the animation boolean parameter for this state</param>
    public PlayerChannelingSpellStartState(Player player, StateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    /// <summary>
    /// Called when entering the channeling start state
    /// </summary>
    public override void Enter()
    {
        base.Enter();
    }

    /// <summary>
    /// Called when exiting the channeling start state
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// Updates the state's logic
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Ensure player remains stationary during animation
        Movement?.SetVelocityX(0f);

        if (!isExitingState)
        {
            // If player releases button early during start animation
            if (!player.InputHandler.SpellCastInput)
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }

    /// <summary>
    /// Called when the start animation finishes
    /// </summary>
    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        if (player.InputHandler.SpellCastInput)
        {
            stateMachine.ChangeState(player.ActiveChannelingSpellState);
        }
        else
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
