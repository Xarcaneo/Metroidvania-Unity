using UnityEngine;

/// <summary>
/// Represents the Oculon enemy type, a floating eye-like creature that performs ranged attacks.
/// </summary>
public class Oculon : Enemy
{
    #region States
    public Oculon_RangedAttackState rangedAttackState { get; private set; }
    public Oculon_MoveState moveState { get; private set; }
    public Oculon_DeathState deathState { get; private set; }
    #endregion

    #region State Configuration Data
    [Header("State Configuration")]
    [SerializeField] private D_RangedAttackState rangedAttackData;
    [SerializeField] private D_AttackCooldownState attackCooldownData;
    [SerializeField] private D_MoveState moveStateData;
    #endregion

    #region References
    [Header("Transform References")]
    [SerializeField] private Transform attackPosition;
    [SerializeField] private Transform[] targetPoints;
    #endregion

    /// <summary>
    /// Initializes the Oculon's states during Awake.
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        InitializeStates();
    }

    /// <summary>
    /// Starts the Oculon with its initial move state.
    /// </summary>
    public override void Start()
    {
        base.Start();
        StateMachine.Initialize(moveState);
    }

    /// <summary>
    /// Returns the death state for this enemy.
    /// </summary>
    /// <returns>The Oculon's death state</returns>
    public override State GetDeathState()
    {
        return deathState;
    }

    /// <summary>
    /// Creates instances of all states with their respective configurations.
    /// </summary>
    private void InitializeStates()
    {
        rangedAttackState = new Oculon_RangedAttackState(this, StateMachine, "attack", rangedAttackData, attackPosition);
        moveState = new Oculon_MoveState(this, StateMachine, "move", moveStateData, targetPoints);
        deathState = new Oculon_DeathState(this, StateMachine, "death", this);
    }
}
