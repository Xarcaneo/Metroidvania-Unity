using UnityEngine.InputSystem;

/// <summary>
/// Interface for input handlers that process different input device types
/// </summary>
public interface IInputHandler
{
    void ProcessInput(InputAction.CallbackContext context, InputActionType actionType);
    void Update();
}
