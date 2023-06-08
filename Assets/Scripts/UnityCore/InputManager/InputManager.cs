using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;

    public static InputManager Instance { get => _instance; }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    [SerializeField] public PlayerInput menuInput;

    public bool isInputActive = true;

    private void Start() => GameEvents.Instance.onDialogueTrigger += OnDialogueTrigger;
    private void OnDestroy() => GameEvents.Instance.onDialogueTrigger -= OnDialogueTrigger;

    private void Update()
    {
        MenuInputUpdate();
    }

    public event Action OnMenuReturn;
    public event Action OnMenuPlayerMenu;
    public event Action OnMenuPreviousTab;
    public event Action OnMenuNextTab;
    public event Action OnMenuDelete;

    private void MenuInputUpdate()
    {
        if (isInputActive)
        {
            if (menuInput.actions["Return"].triggered) OnMenuReturn?.Invoke();
            else if (menuInput.actions["PlayerMenu"].triggered) OnMenuPlayerMenu?.Invoke();
            else if (menuInput.actions["PreviousTab"].triggered) OnMenuPreviousTab?.Invoke();
            else if (menuInput.actions["NextTab"].triggered) OnMenuNextTab?.Invoke();
            else if (menuInput.actions["Delete"].triggered) OnMenuDelete?.Invoke();
        }

        if (menuInput.actions["Console"].triggered) ToggleConsole();
    }

    private void OnDialogueTrigger( bool dialogueState )
    {
        isInputActive = !dialogueState;
    }

    public Vector2 GetNavigateValue()
    {
        return menuInput.actions["Navigate"].ReadValue<Vector2>();
    }

    private void ToggleConsole()
    {
        if(QuantumConsole.Instance.canToggleConsole)
            QuantumConsole.Instance.Toggle();

        GameEvents.Instance.DeactivatePlayerInput(QuantumConsole.Instance.IsActive);
        isInputActive = !QuantumConsole.Instance.IsActive;
    }
}
