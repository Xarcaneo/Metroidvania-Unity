using UnityEngine;
using PixelCrushers.DialogueSystem;

/// <summary>
/// Checks a Dialogue System variable on Awake and deactivates the parent GameObject
/// if the variable matches the expected boolean value.
/// </summary>
public class DialogueVariableChecker : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The name of the Dialogue System variable to check")]
    private string variableName;

    [SerializeField]
    [Tooltip("The expected value of the variable. If the variable matches this value, the parent will be deactivated")]
    private bool expectedValue = true;

    private void Awake()
    {
        if (string.IsNullOrEmpty(variableName))
        {
            Debug.LogWarning($"[{nameof(DialogueVariableChecker)}] Variable name is empty!", this);
            return;
        }

        // Get the variable value from Dialogue System
        bool currentValue = DialogueLua.GetVariable(variableName).asBool;

        // If the value matches our expectation, deactivate the parent
        if (currentValue == expectedValue && transform.parent != null)
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
}
