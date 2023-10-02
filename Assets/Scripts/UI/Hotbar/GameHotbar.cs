using Opsive.UltimateInventorySystem.UI.Item;
using Opsive.UltimateInventorySystem.UI.Panels.Hotbar;
using System.Collections.Generic;
using UnityEngine;

public class GameHotbar : MonoBehaviour
{
    [SerializeField] private ItemHotbar m_itemHotbar;
    [SerializeField] private int itemSlotIndex = 0;

    private int maxIndex = 0;

    private Dictionary<int, ItemViewSlot> itemViewSlotDictionary = new Dictionary<int, ItemViewSlot>();

    private PlayerInputHandler m_playerInputHandler;

    private void Awake()
    {
        Transform[] childTransforms = GetComponentsInChildren<Transform>();

        foreach (Transform childTransform in childTransforms)
        {
            ItemViewSlot itemViewSlot = childTransform.GetComponent<ItemViewSlot>();

            if (itemViewSlot != null)
            {
                itemViewSlotDictionary[maxIndex] = itemViewSlot;
                maxIndex++;
            }
        }

        maxIndex -= 1;
    }

    private void Update()
    {
        if (m_playerInputHandler == null)
        {
            m_playerInputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputHandler>();
            FocusSlot(itemSlotIndex);
        }
        else
        {
            var hotbarActionInput = m_playerInputHandler.HotbarActionInput;
            var itemSwitchLeftInput = m_playerInputHandler.ItemSwitchLeftInput;
            var itemSwitchRightInput = m_playerInputHandler.ItemSwitchRightInput;

            if (hotbarActionInput)
            {
                m_itemHotbar.UseItem(itemSlotIndex);
                m_playerInputHandler.UseHotbarActionInput();
            }

            if (itemSwitchRightInput)
            {
                if (itemSlotIndex >= maxIndex) itemSlotIndex = 0;
                else itemSlotIndex++;

                m_playerInputHandler.UseItemSwitchRightInput();

                FocusSlot(itemSlotIndex);
            }

            if (itemSwitchLeftInput)
            {
                if (itemSlotIndex <= 0) itemSlotIndex = maxIndex;
                else itemSlotIndex--;

                m_playerInputHandler.UseItemSwitchLeftInput();

                FocusSlot(itemSlotIndex);
            }
        }
    }

    private void FocusSlot(int index)
    {
        if (itemViewSlotDictionary.ContainsKey(index))
        {
            ItemViewSlot itemViewSlot = itemViewSlotDictionary[index];

            // Call the method on the ItemViewSlot
            itemViewSlot.Select();
        }
    }
}
