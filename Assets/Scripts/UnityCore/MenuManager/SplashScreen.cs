using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the behavior of the splash screen, including detecting input to proceed to the main menu.
/// </summary>
public class SplashScreen : MonoBehaviour
{
    /// <summary>
    /// Reference to the PlayerInput component to detect user input.
    /// </summary>
    private PlayerInput menuInput;

    /// <summary>
    /// Cached reference to the return input action for better performance.
    /// </summary>
    private InputAction returnAction;

    /// <summary>
    /// Initializes the PlayerInput component and caches the return input action.
    /// </summary>
    private void Start()
    {
        menuInput = GetComponent<PlayerInput>();
        returnAction = menuInput.actions["Return"];
    }

    /// <summary>
    /// Continuously checks for the return input to proceed to the main menu.
    /// </summary>
    private void Update()
    {
        if (returnAction.triggered)
        {
            menuInput.DeactivateInput();
            SaveSystem.LoadScene("MainMenu");
        }
    }
}
