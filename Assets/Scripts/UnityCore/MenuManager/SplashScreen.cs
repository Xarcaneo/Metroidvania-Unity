using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SplashScreen : MonoBehaviour
{
    private PlayerInput menuInput;

    private void Start()
    {
        menuInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        OnReturnInput();
    }

    public virtual void OnReturnInput()
    {
        if (menuInput.actions["Return"].triggered)
        {
            menuInput.DeactivateInput();
            SaveSystem.LoadScene("MainMenu");
        }
    }
}
