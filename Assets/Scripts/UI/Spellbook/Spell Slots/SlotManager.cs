using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Manages slot focus within a specific canvas.
/// Ensures the correct slot is focused when the canvas is activated.
/// </summary>
public class SlotManager : MonoBehaviour
{
    /// <summary>
    /// The parent container holding all slot buttons for this canvas.
    /// </summary>
    [SerializeField] private Transform gridContainer;

    /// <summary>
    /// The first slot to focus when this canvas is activated.
    /// </summary>
    [SerializeField] private Slot firstSlot;

    /// <summary>
    /// Called when the canvas is enabled. Focuses the first slot.
    /// </summary>
    private void OnEnable()
    {
        if (firstSlot == null)
        {
            Debug.LogWarning("First slot is not assigned.");
            return;
        }

        EventSystem.current.SetSelectedGameObject(firstSlot.gameObject);

        // Trigger custom select logic if necessary
        Slot slotComponent = firstSlot;
        if (slotComponent != null)
        {
            slotComponent.OnSelect(null);
        }
    }
}
