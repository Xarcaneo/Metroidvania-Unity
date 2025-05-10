using UnityEngine;
using Opsive.UltimateInventorySystem.UI.Item;
using UnityCore.AudioManager;
using UnityEngine.EventSystems;

/// <summary>
/// Static class to manage inventory audio state
/// </summary>
public static class InventoryAudioState
{
    public static bool IsFirstSelection { get; private set; } = true;

    public static void Reset()
    {
        IsFirstSelection = true;
    }

    public static void MarkSelectionMade()
    {
        IsFirstSelection = false;
    }
}

/// <summary>
/// Handles audio feedback for ItemViewSlot interactions.
/// Attach this component to the same GameObject as ItemViewSlot.
/// </summary>
[RequireComponent(typeof(ItemViewSlot))]
public class ItemViewSlotAudioHandler : MonoBehaviour, ISelectHandler
{
    private ItemViewSlot m_ItemViewSlot;

    [Header("Audio Settings")]
    [Tooltip("Sound played when selecting the slot")]
    [SerializeField] private AudioEventId m_SelectSound = AudioEventId.UI_Slot_Navigate;

    private void Awake()
    {
        m_ItemViewSlot = GetComponent<ItemViewSlot>();
        if (m_ItemViewSlot == null)
        {
            Debug.LogError($"[ItemViewSlotAudioHandler] No ItemViewSlot found on {gameObject.name}", this);
            enabled = false;
            return;
        }
    }



    public void OnSelect(BaseEventData eventData)
    {
        // Play select sound when slot is selected (except for first selection)
        if (!InventoryAudioState.IsFirstSelection)
        {
            AudioManager.instance.PlaySound(m_SelectSound);
        }
        InventoryAudioState.MarkSelectionMade();
    }
}
