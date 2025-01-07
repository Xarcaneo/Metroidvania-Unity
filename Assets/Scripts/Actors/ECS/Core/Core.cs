using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Core component manager for entities in the game.
/// Acts as a central hub for managing and accessing various core components attached to an entity.
/// </summary>
public class Core : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// List of all core components managed by this core.
    /// </summary>
    private readonly List<CoreComponent> CoreComponents = new List<CoreComponent>();

    /// <summary>
    /// Reference to the parent entity that owns this core.
    /// </summary>
    public Entity Parent { get; private set; }

    #endregion

    #region Unity Callback Methods

    /// <summary>
    /// Initializes the core by finding its parent entity.
    /// </summary>
    private void Start()
    {
        Parent = GetComponentInParent<Entity>();
    }

    #endregion

    #region Component Management

    /// <summary>
    /// Updates the logic of all core components.
    /// Called during the entity's update cycle.
    /// </summary>
    public void LogicUpdate()
    {
        foreach (CoreComponent component in CoreComponents)
        {
            component.LogicUpdate();
        }
    }

    /// <summary>
    /// Adds a new core component to this core if it doesn't already exist.
    /// </summary>
    /// <param name="component">The component to add</param>
    public void AddComponent(CoreComponent component)
    {
        if (!CoreComponents.Contains(component))
        {
            CoreComponents.Add(component);
        }
    }

    /// <summary>
    /// Retrieves a core component of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of component to retrieve</typeparam>
    /// <returns>The first component of the specified type, or null if not found</returns>
    public T GetCoreComponent<T>() where T : CoreComponent
    {
        return CoreComponents.OfType<T>().FirstOrDefault();
    }

    /// <summary>
    /// Retrieves a core component and assigns it to the provided reference.
    /// </summary>
    /// <typeparam name="T">The type of component to retrieve</typeparam>
    /// <param name="value">Reference to store the retrieved component</param>
    /// <returns>The retrieved component</returns>
    public T GetCoreComponent<T>(ref T value) where T : CoreComponent
    {
        value = GetCoreComponent<T>();
        return value;
    }

    #endregion
}