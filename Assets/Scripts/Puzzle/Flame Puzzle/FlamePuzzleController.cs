using UnityEngine;
using System;

public class FlamePuzzleController : MonoBehaviour
{
    [SerializeField] private FlameSlot[] flameSlots;
    [SerializeField] private bool requireSpecificOrder = false;
    [SerializeField] private int[] correctOrder;

    private int activatedSlotsCount = 0;
    private int maxPoints;
    private int currentOrderIndex = 0;

    public event Action<float> OnProgressChanged;
    public event Action OnPuzzleReset;

    private void Start()
    {
        maxPoints = flameSlots.Length;

        if (requireSpecificOrder && (correctOrder == null || correctOrder.Length != maxPoints))
        {
            Debug.LogError("Correct order array must be set and match the number of flame slots when specific order is required!");
            enabled = false;
            return;
        }

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

    private void OnDestroy()
    {
        foreach (var slot in flameSlots)
        {
            if (slot != null)
            {
                slot.OnStateChanged -= OnSlotStateChanged;
            }
        }
    }

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
            activatedSlotsCount = Mathf.Max(0, activatedSlotsCount - 1);
            OnProgressChanged?.Invoke((float)activatedSlotsCount / maxPoints);
        }
    }

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
        OnProgressChanged?.Invoke((float)activatedSlotsCount / maxPoints);

        if (activatedSlotsCount >= maxPoints)
        {
            FlamePuzzleManager.Instance.OnPuzzleCompleted();
        }
    }

    private void HandleUnorderedActivation()
    {
        activatedSlotsCount++;
        OnProgressChanged?.Invoke((float)activatedSlotsCount / maxPoints);

        if (activatedSlotsCount >= maxPoints)
        {
            FlamePuzzleManager.Instance.OnPuzzleCompleted();
        }
    }

    private bool IsSlotInCorrectOrder(FlameSlot slot)
    {
        int slotIndex = Array.IndexOf(flameSlots, slot);
        return slotIndex >= 0 && slotIndex < currentOrderIndex && correctOrder[slotIndex] == slotIndex;
    }

    private void ResetPuzzle()
    {
        foreach (var slot in flameSlots)
        {
            if (slot != null && slot.IsActivated)
            {
                // Use reflection to access private method if needed
                var resetMethod = slot.GetType().GetMethod("DeactivateSlot", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                resetMethod?.Invoke(slot, null);
            }
        }

        activatedSlotsCount = 0;
        currentOrderIndex = 0;
        OnProgressChanged?.Invoke(0f);
        OnPuzzleReset?.Invoke();
    }
}
