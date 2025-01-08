using UnityEngine;

/// <summary>
/// Handles the death behavior for the Reaver enemy.
/// Manages death animation, messaging system notification, and cleanup.
/// </summary>
public class Reaver_DeathState : DeathState
{
    private readonly Reaver enemy;
    private const string ENEMY_TYPE = "Reaver";

    /// <summary>
    /// Initializes a new instance of the Reaver_DeathState class.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">State machine managing this state</param>
    /// <param name="animBoolName">Animation boolean parameter name</param>
    public Reaver_DeathState(Entity entity, StateMachine stateMachine, string animBoolName) 
        : base(entity, stateMachine, animBoolName)
    {
        this.enemy = entity as Reaver;
    }

    /// <summary>
    /// Called when entering the death state.
    /// Notifies the messaging system about the Reaver's death.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        PixelCrushers.MessageSystem.SendMessage(this, "Killed", ENEMY_TYPE);
    }

    /// <summary>
    /// Updates the logical state of the death behavior.
    /// Handles cleanup after death animation finishes.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            EnemyDeath.Die();
            enemy.gameObject.SetActive(false);
        }
    }
}
