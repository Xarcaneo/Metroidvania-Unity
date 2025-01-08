using UnityEngine;
using System;

/// <summary>
/// Controls the logic for a flame-based puzzle where players must activate flame slots in a specific order or any order.
/// </summary>
public class FlamePuzzleController : MonoBehaviour
{
    [Header("Puzzle Configuration")]
    [Tooltip("Array of flame slots that make up this puzzle")]
    [SerializeField] private FlameSlot[] flameSlots;
    
    [Tooltip("Whether flames must be activated in a specific order")]
    [SerializeField] private bool requireSpecificOrder = false;
    
    [Tooltip("The correct order of flame activation (only used if requireSpecificOrder is true)")]
    [SerializeField] private int[] correctOrder;

    private int activatedSlotsCount = 0;
    private int maxPoints;
    private int currentOrderIndex = 0;

    /// <summary>
    /// Event triggered when puzzle progress changes. Float parameter represents completion percentage (0-1).
    /// </summary>
    public event Action<float> OnProgressChanged;

    /// <summary>
    /// Event triggered when the puzzle needs to be reset.
    /// </summary>
    public event Action OnPuzzleReset;

    /// <summary>
    /// Initializes the puzzle and sets up event subscriptions.
    /// </summary>
    private void Start()
    {
        InitializePuzzle();
    }

    /// <summary>
    /// Cleans up event subscriptions when the component is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    /// <summary>
    /// Initializes the puzzle by validating configuration and setting up event handlers.
    /// </summary>
    private void InitializePuzzle()
    {
        maxPoints = flameSlots.Length;

        if (requireSpecificOrder && !ValidateOrderArray())
        {
            Debug.LogError("Correct order array must be set and match the number of flame slots when specific order is required!");
            enabled = false;
            return;
        }

        SubscribeToEvents();
    }

    /// <summary>
    /// Validates that the correct order array is properly configured.
    /// </summary>
    /// <returns>True if the order array is valid, false otherwise</returns>
    private bool ValidateOrderArray()
    {
        return correctOrder != null && correctOrder.Length == maxPoints;
    }

    /// <summary>
    /// Subscribes to state change events for all flame slots.
    /// </summary>
    private void SubscribeToEvents()
    {
        foreach (var slot in flameSlots)
        {
            if (slot != null)
            {
                slot.OnStateChanged += OnSlotStateChanged;
            }
            else
            {
                Debug.LogWarning("Null FlameSlot reference found in FlamePuzzleController!");
            }
        }
    }

    /// <summary>
    /// Unsubscribes from all flame slot events to prevent memory leaks.
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        foreach (var slot in flameSlots)
        {
            if (slot != null)
            {
                slot.OnStateChanged -= OnSlotStateChanged;
            }
        }
    }

    /// <summary>
    /// Handles state changes of individual flame slots.
    /// </summary>
    /// <param name="activated">True if the slot was activated, false if deactivated</param>
    private void OnSlotStateChanged(bool activated)
    {
        if (activated)
        {
            if (requireSpecificOrder)
            {
                HandleOrderedActivation();
            }
            else
            {
                HandleUnorderedActivation();
            }
        }
        else
        {
            HandleDeactivation();
        }
    }

    /// <summary>
    /// Handles flame activation when a specific order is required.
    /// Resets the puzzle if flames are activated in the wrong order.
    /// </summary>
    private void HandleOrderedActivation()
    {
        FlameSlot activatedSlot = Array.Find(flameSlots, slot => slot.IsActivated && !IsSlotInCorrectOrder(slot));
        
        if (activatedSlot != null)
        {
            ResetPuzzle();
            return;
        }

        activatedSlotsCount++;
        currentOrderIndex++;
        UpdateProgress();

        CheckPuzzleCompletion();
    }

    /// <summary>
    /// Handles flame activation when no specific order is required.
    /// </summary>
    private void HandleUnorderedActivation()
    {
        activatedSlotsCount++;
        UpdateProgress();

        CheckPuzzleCompletion();
    }

    /// <summary>
    /// Handles flame deactivation, updating progress accordingly.
    /// </summary>
    private void HandleDeactivation()
    {
        activatedSlotsCount = Mathf.Max(0, activatedSlotsCount - 1);
        UpdateProgress();
    }

    /// <summary>
    /// Updates and broadcasts the current puzzle progress.
    /// </summary>
    private void UpdateProgress()
    {
        OnProgressChanged?.Invoke((float)activatedSlotsCount / maxPoints);
    }

    /// <summary>
    /// Checks if all slots are activated and triggers puzzle completion if they are.
    /// </summary>
    private void CheckPuzzleCompletion()
    {
        if (activatedSlotsCount >= maxPoints)
        {
            FlamePuzzleManager.Instance.OnPuzzleCompleted();
        }
    }

    /// <summary>
    /// Checks if a flame slot is activated in the correct order.
    /// </summary>
    /// <param name="slot">The flame slot to check</param>
    /// <returns>True if the slot is in the correct order or if order doesn't matter</returns>
    private bool IsSlotInCorrectOrder(FlameSlot slot)
    {
        if (!requireSpecificOrder || correctOrder == null) return true;

        int slotIndex = Array.IndexOf(flameSlots, slot);
        return slotIndex >= 0 && slotIndex < correctOrder.Length && correctOrder[slotIndex] == currentOrderIndex;
    }

    /// <summary>
    /// Resets the puzzle state, deactivating all flames and resetting progress.
    /// </summary>
    private void ResetPuzzle()
    {
        activatedSlotsCount = 0;
        currentOrderIndex = 0;
        UpdateProgress();
        OnPuzzleReset?.Invoke();
    }
}
