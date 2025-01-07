using UnityEngine;

/// <summary>
/// Interface for components that need to perform logic updates.
/// Separates logic updates from Unity's MonoBehaviour Update to allow for better control over update timing.
/// </summary>
public interface ILogicUpdate
{
    /// <summary>
    /// Performs logic updates for the component.
    /// Called independently of Unity's Update cycle.
    /// </summary>
    void LogicUpdate();
}