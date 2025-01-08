using UnityEngine;

/// <summary>
/// Handles the dash behavior for the Optikira enemy.
/// During dash, the enemy becomes temporarily invulnerable and moves quickly in a direction.
/// </summary>
public class Optikira_DashState : DashState
{
    private readonly Optikira enemy;
    
    // Cached components
    private HurtArea HurtArea { get => hurtArea ?? core.GetCoreComponent(ref hurtArea); }
    private HurtArea hurtArea;

    /// <summary>
    /// Initializes a new instance of the Optikira_DashState class.
    /// </summary>
    /// <param name="entity">The entity this state belongs to</param>
    /// <param name="stateMachine">State machine managing this state</param>
    /// <param name="animBoolName">Animation boolean parameter name</param>
    /// <param name="stateData">Configuration data for the dash state</param>
    public Optikira_DashState(Entity entity, StateMachine stateMachine, string animBoolName, D_DashState stateData) 
        : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = (Optikira)entity;
    }

    /// <summary>
    /// Called when entering the dash state.
    /// Disables hurt area and determines dash direction based on collision checks.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        HurtArea.isActive = false;
        DetermineDashDirection();
    }

    /// <summary>
    /// Called when exiting the dash state.
    /// Re-enables hurt area.
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        HurtArea.isActive = true;
    }

    /// <summary>
    /// Updates the logical state of the dash behavior.
    /// Handles movement and state transitions.
    /// </summary>
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            PerformDash();
            CheckDashComplete();
        }
    }

    /// <summary>
    /// Determines the direction for the dash based on collision checks.
    /// </summary>
    private void DetermineDashDirection()
    {
        dashDirection = DashBackCollision() ? Movement.FacingDirection : -Movement.FacingDirection;
        enemy.Anim.SetInteger("dash_direction", dashDirection);
    }

    /// <summary>
    /// Performs the actual dash movement.
    /// </summary>
    private void PerformDash()
    {
        Movement?.SetVelocity(stateData.dashVelocity, stateData.dashAngle, dashDirection);
    }

    /// <summary>
    /// Checks if the dash should complete and transitions to appropriate state.
    /// </summary>
    private void CheckDashComplete()
    {
        bool isDashComplete = Time.time >= startTime + stateData.dashTime || Movement?.CurrentVelocity.x == 0;
        
        if (isDashComplete)
        {
            stateMachine.ChangeState(isPlayerDetected ? enemy.rangedAttackState : enemy.idleState);
        }
    }
}
