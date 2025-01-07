using UnityEngine;

/// <summary>
/// Base class for all core components in the entity system.
/// Provides common functionality and initialization for components.
/// </summary>
public class CoreComponent : MonoBehaviour
{
    #region Core Reference

    /// <summary>
    /// Reference to the Core component that manages this component.
    /// </summary>
    protected Core core;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Initializes the component by getting the Core reference.
    /// </summary>
    protected virtual void Awake()
    {
        core = transform.parent.GetComponent<Core>();

        if (core == null)
            Debug.LogError($"There is no Core on the parent of {transform.parent.name}");

        core.AddComponent(this);
    }

    #endregion

    #region Component Management

    /// <summary>
    /// Called by Core when all components are ready.
    /// Override this to perform initialization that depends on other components.
    /// </summary>
    public virtual void LogicUpdate() { }

    /// <summary>
    /// Enables or disables this component.
    /// </summary>
    /// <param name="state">True to enable, false to disable</param>
    public virtual void EnableDisableComponent(bool state)
    {
        this.gameObject.SetActive(state);
    }

    #endregion
}