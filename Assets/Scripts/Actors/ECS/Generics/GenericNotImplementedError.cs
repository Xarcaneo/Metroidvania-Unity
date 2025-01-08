using UnityEngine;

/// <summary>
/// Provides generic error handling for unimplemented components or features.
/// </summary>
/// <typeparam name="T">The type of component or value being checked</typeparam>
public static class GenericNotImplementedError<T>
{
    /// <summary>
    /// Checks if a value is implemented (not null) and logs an error if it isn't.
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <param name="name">Name of the object or component for error reporting</param>
    /// <returns>The value if implemented, default(T) if not</returns>
    public static T TryGet(T value, string name)
    {
        if (value != null)
        {
            return value;
        }

        Debug.LogError($"{typeof(T)} not implemented on {name}");
        return default;
    }
}