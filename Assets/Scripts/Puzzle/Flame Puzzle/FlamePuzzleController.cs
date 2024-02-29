using UnityEngine;

public class FlamePuzzleController : MonoBehaviour
{
    [SerializeField] private FlameSlot[] flameSlots;

    private int activatedSlotsCount = 0;
    private int maxPoints; // Will be set automatically based on flameSlots count

    // Start is called before the first frame update
    void Start()
    {
        // Set maxPoints based on the number of flameSlots
        maxPoints = flameSlots.Length;

        // Subscribe to the onActivated event for each FlameSlot
        foreach (var slot in flameSlots)
        {
            if (slot != null)
            {
                slot.IsActivated += OnSlotActivated;
            }
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from the onActivated event for each FlameSlot to clean up
        foreach (var slot in flameSlots)
        {
            if (slot != null)
            {
                slot.IsActivated -= OnSlotActivated;
            }
        }
    }

    // Callback method for the onActivated event
    void OnSlotActivated()
    {
        // Increase the activated slots count
        activatedSlotsCount++;

        // Check if the maximum points are reached
        if (activatedSlotsCount >= maxPoints)
        {
            FlamePuzzleManager.Instance.OnPuzzleCompleted();
        }
        else
        {
            Debug.Log("Activated slots: " + activatedSlotsCount);
        }
    }
}
