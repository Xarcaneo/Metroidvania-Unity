using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset actions;

    public int currentSaveSlot;
    public bool shouldFlipPlayer = false;

    #region Instance variables
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; }
    #endregion

    #region Initialize functions
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

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    #endregion

    #region Input
    private void OnDeviceChange(InputDevice arg1, InputDeviceChange arg2)
    {
        UpdateBindingToDialogue();
    }

    public void UpdateBindingToDialogue()
    {
        // Iterate through all action maps in the asset
        foreach (InputActionMap actionMap in actions.actionMaps)
        {
            // Iterate through all actions in the action map
            foreach (InputAction action in actionMap.actions)
            {
                // Iterate through all bindings of the action
                foreach (InputBinding binding in action.bindings)
                {
                    // Get the display string for the binding with only the key
                    string keyName = binding.ToDisplayString(
                        InputBinding.DisplayStringOptions.DontIncludeInteractions);

                    DialogueLua.SetVariable(action.name + "_Keybinding", keyName);
                }
            }
        }
    }

    #endregion
}
