using UnityEngine;

/// <summary>
/// Utility class providing generic component validation methods for Unity components.
/// </summary>
public static class ComponentValidationUtility
{
    private const string VALIDATOR_TAG = "[VALIDATOR]";

    /// <summary>
    /// Controls when validation should occur
    /// </summary>
    /// <param name="component">The MonoBehaviour requesting validation</param>
    /// <returns>True if validation should proceed, false if it should be skipped</returns>
    public static bool ShouldValidate(MonoBehaviour component)
    {
        // Skip validation in edit mode or if the game object is not active
        if (!Application.isPlaying || !component.gameObject.activeInHierarchy)
            return false;

        // Skip validation if we're still in initialization (first frame)
        if (Time.frameCount <= 1)
            return false;

        return true;
    }

    /// <summary>
    /// Validates if a required component exists on a GameObject.
    /// </summary>
    /// <typeparam name="T">Type of component to validate</typeparam>
    /// <param name="component">The component to validate</param>
    /// <param name="gameObject">GameObject that should have the component</param>
    /// <param name="componentName">Optional custom name for the component in error messages</param>
    /// <returns>True if component exists and is valid, false otherwise</returns>
    public static bool ValidateRequiredComponent<T>(T component, GameObject gameObject, string componentName = null) where T : Component
    {
        if (component == null)
        {
            string name = componentName ?? typeof(T).Name;
            Debug.LogError($"{VALIDATOR_TAG} [{gameObject.name}] {name} component is missing!");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Validates if a required component exists in the scene.
    /// </summary>
    /// <typeparam name="T">Type of component to validate</typeparam>
    /// <param name="component">The component to validate</param>
    /// <param name="callerObject">GameObject that's requesting the component</param>
    /// <param name="componentName">Optional custom name for the component in error messages</param>
    /// <returns>True if component exists and is valid, false otherwise</returns>
    public static bool ValidateRequiredSceneComponent<T>(T component, GameObject callerObject, string componentName = null) where T : Component
    {
        if (component == null)
        {
            string name = componentName ?? typeof(T).Name;
            Debug.LogError($"{VALIDATOR_TAG} [{callerObject.name}] {name} not found in scene!");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Validates if a string field is properly set.
    /// </summary>
    /// <param name="value">String value to validate</param>
    /// <param name="gameObject">GameObject that owns the field</param>
    /// <param name="fieldName">Name of the field for error messages</param>
    /// <param name="logAsWarning">If true, logs as warning instead of error</param>
    /// <returns>True if string is valid, false otherwise</returns>
    public static bool ValidateRequiredString(string value, GameObject gameObject, string fieldName, bool logAsWarning = false)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (logAsWarning)
                Debug.LogWarning($"{VALIDATOR_TAG} [{gameObject.name}] {fieldName} is not set!");
            else
                Debug.LogError($"{VALIDATOR_TAG} [{gameObject.name}] {fieldName} is not set!");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Validates if an array or list has required elements.
    /// </summary>
    /// <param name="count">Number of elements in the collection</param>
    /// <param name="gameObject">GameObject that owns the collection</param>
    /// <param name="collectionName">Name of the collection for error messages</param>
    /// <param name="logAsWarning">If true, logs as warning instead of error</param>
    /// <returns>True if collection has elements, false otherwise</returns>
    public static bool ValidateCollectionNotEmpty(int count, GameObject gameObject, string collectionName, bool logAsWarning = false)
    {
        if (count == 0)
        {
            if (logAsWarning)
                Debug.LogWarning($"{VALIDATOR_TAG} [{gameObject.name}] {collectionName} has no elements!");
            else
                Debug.LogError($"{VALIDATOR_TAG} [{gameObject.name}] {collectionName} has no elements!");
            return false;
        }
        return true;
    }
}
