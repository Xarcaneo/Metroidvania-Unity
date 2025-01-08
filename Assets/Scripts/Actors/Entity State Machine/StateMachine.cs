using UnityEngine;

/// <summary>
/// Manages state transitions for an entity.
/// Handles the initialization and switching between different states.
/// </summary>
public class StateMachine
{
    /// <summary>
    /// Gets the currently active state.
    /// </summary>
    public State CurrentState { get; private set; }

    /// <summary>
    /// Initializes the state machine with a starting state.
    /// </summary>
    /// <param name="startingState">The initial state to enter</param>
    public void Initialize(State startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    /// <summary>
    /// Changes the current state to a new state.
    /// Handles proper exit and entry of states.
    /// </summary>
    /// <param name="newState">The new state to transition to</param>
    public void ChangeState(State newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}