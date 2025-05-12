using UnityEngine;

/// <summary>
/// Handles the ranged attack behavior for the Oculon enemy.
/// The Oculon performs unblockable ranged attacks and faces the player during combat.
/// </summary>
public class Oculon_RangedAttackState : RangedAttackState
{
    private readonly Oculon enemy;

    /// <summary>
    /// Initializes a new instance of the Oculon_RangedAttackState class.
    /// </summary>
    /// <param name="entity">The Oculon entity</param>
    /// <param name="stateMachine">State machine managing this state</param>
    /// <param name="animBoolName">Animation boolean parameter name</param>
    /// <param name="stateData">Ranged attack configuration data</param>
    /// <param name="attackPosition">Transform marking the projectile spawn point</param>
    public Oculon_RangedAttackState(Entity entity, StateMachine stateMachine, string animBoolName, D_RangedAttackState stateData, Transform attackPosition) 
        : base(entity, stateMachine, animBoolName, stateData, attackPosition)
    {
        this.enemy = (Oculon)entity;
    }

    /// <summary>
    /// Called when entering the ranged attack state.
    /// Sets up the attack properties, making it unblockable.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        m_damageData.CanParry = false;
    }

    /// <summary>
    /// Updates the logical state of the ranged attack behavior.
    /// Handles facing the player and transitioning back to move state after attack.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Face the player
        if (EntityDetector.entityToRight != Movement.FacingDirection)
        {
            Movement.Flip();
        }

        // Return to move state after attack animation
        if (isAnimationFinished)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}
