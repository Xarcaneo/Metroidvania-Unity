using UnityEngine;

/// <summary>
/// Handles the ranged attack behavior for the Optikira enemy.
/// The Optikira performs unblockable ranged attacks while facing the player.
/// </summary>
public class Optikira_RangedAttackState : RangedAttackState
{
    private readonly Optikira enemy;

    /// <summary>
    /// Initializes a new instance of the Optikira_RangedAttackState class.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">State machine managing this state</param>
    /// <param name="animBoolName">Animation boolean parameter name</param>
    /// <param name="stateData">Configuration data for the ranged attack</param>
    /// <param name="attackPosition">Transform marking the projectile spawn point</param>
    public Optikira_RangedAttackState(Entity entity, StateMachine stateMachine, string animBoolName, D_RangedAttackState stateData, Transform attackPosition) 
        : base(entity, stateMachine, animBoolName, stateData, attackPosition)
    {
        this.enemy = (Optikira)entity;
    }

    /// <summary>
    /// Called when entering the ranged attack state.
    /// Sets up the attack properties, making it unblockable.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        m_damageData.CanBlock = false;
    }

    /// <summary>
    /// Updates the logical state of the ranged attack behavior.
    /// Handles facing the player and transitioning to cooldown after attack.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        UpdateFacing();
        CheckAttackComplete();
    }

    /// <summary>
    /// Updates the enemy's facing direction to match player position.
    /// </summary>
    private void UpdateFacing()
    {
        if (EntityDetector.entityToRight != Movement.FacingDirection)
        {
            Movement.Flip();
        }
    }

    /// <summary>
    /// Checks if the attack is complete and transitions to cooldown state.
    /// </summary>
    private void CheckAttackComplete()
    {
        if (isAnimationFinished)
        {
            stateMachine.ChangeState(enemy.attackCooldownState);
        }
    }
}
