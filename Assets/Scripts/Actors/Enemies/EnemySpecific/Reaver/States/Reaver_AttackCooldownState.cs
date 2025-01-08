using UnityEngine;

/// <summary>
/// Handles the cooldown period after a Reaver attack.
/// During this state, the enemy waits before transitioning to chase or patrol state.
/// </summary>
public class Reaver_AttackCooldownState : AttackCooldownState
{
    private readonly Reaver enemy;

    // Cached components
    private EnemyDamageHitBox EnemyDamageHitBox { get => enemyDamageHitBox ?? core.GetCoreComponent(ref enemyDamageHitBox); }
    private EnemyDamageHitBox enemyDamageHitBox;

    /// <summary>
    /// Initializes a new instance of the Reaver_AttackCooldownState class.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">State machine managing this state</param>
    /// <param name="animBoolName">Animation boolean parameter name</param>
    /// <param name="stateData">Configuration data for the cooldown state</param>
    public Reaver_AttackCooldownState(Entity entity, StateMachine stateMachine, string animBoolName, D_AttackCooldownState stateData) 
        : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = (Reaver)entity;
    }

    private bool attackableTargetDetected;

    /// <summary>
    /// Performs environmental checks and player detection.
    /// </summary>
    public override void DoChecks()
    {
        base.DoChecks();
        attackableTargetDetected = EnemyDamageHitBox.EntityInRange();
    }

    /// <summary>
    /// Updates the logical state of the cooldown behavior.
    /// Handles transitions to chase or patrol states after cooldown.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (timeSinceEnteredState >= stateData.cooldownTime)
        {
            if (attackableTargetDetected)
            {
                stateMachine.ChangeState(enemy.meleeAttackState);
            }
            else
            {
                stateMachine.ChangeState(enemy.patrolState);
            }
        }
    }
}
