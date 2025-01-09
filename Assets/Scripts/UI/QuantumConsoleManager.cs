using UnityEngine;
using UnityEngine.EventSystems;
using QFSW.QC;

/// <summary>
/// Manages the integration between Quantum Console and Unity's UI system,
/// particularly handling focus restoration when the console is closed.
/// </summary>
public class QuantumConsoleManager : MonoBehaviour
{
    private QuantumConsole console;
    private GameObject lastSelectedGameObject;

    private void Awake()
    {
        console = GetComponent<QuantumConsole>();
        if (console == null)
        {
            Debug.LogError("QuantumConsoleManager requires a QuantumConsole component!");
            return;
        }

        // Subscribe to console events
        console.OnActivate += OnConsoleOpened;
        console.OnDeactivate += OnConsoleClosed;
    }

    private void OnDestroy()
    {
        if (console != null)
        {
            // Unsubscribe from console events
            console.OnActivate -= OnConsoleOpened;
            console.OnDeactivate -= OnConsoleClosed;
        }
    }

    private void Update()
    {
        // Store the currently selected UI element continuously when console is not active
        if (console != null && !console.IsActive)
        {
            var currentSelected = EventSystem.current?.currentSelectedGameObject;
            if (currentSelected != null && currentSelected != console.gameObject)
            {
                lastSelectedGameObject = currentSelected;
            }
        }
    }

    private void OnConsoleOpened()
    {
    }

    private void OnConsoleClosed()
    {
        // Restore focus to the last selected UI element
        if (lastSelectedGameObject != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedGameObject);
            
            // Trigger select event on the UI element
            var selectHandler = lastSelectedGameObject.GetComponent<ISelectHandler>();
            if (selectHandler != null)
            {
                selectHandler.OnSelect(new BaseEventData(EventSystem.current));
            }
        }
    }
}
