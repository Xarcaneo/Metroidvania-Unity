using Opsive.UltimateInventorySystem.UI.Item;
using Opsive.UltimateInventorySystem.UI.Panels.Hotbar;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the game's hotbar system for quick item access.
/// Handles item slot selection, navigation, and usage.
/// </summary>
public class GameHotbar : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>
    /// Reference to the Ultimate Inventory System's item hotbar.
    /// </summary>
    [SerializeField] private ItemHotbar m_itemHotbar;

    /// <summary>
    /// Currently selected item slot index.
    /// </summary>
    [SerializeField] private int itemSlotIndex = 0;
    #endregion

    #region Private Fields
    /// <summary>
    /// Maximum index of available item slots.
    /// </summary>
    private int maxIndex = 0;

    /// <summary>
    /// Maps slot indices to their corresponding UI view slots.
    /// </summary>
    private Dictionary<int, ItemViewSlot> itemViewSlotDictionary = new Dictionary<int, ItemViewSlot>();

    /// <summary>
    /// Reference to the player's input handler.
    /// </summary>
    private PlayerInputHandler m_playerInputHandler;
    #endregion

    #region Unity Event Functions
    /// <summary>
    /// Initializes the hotbar by finding and mapping all item view slots.
    /// </summary>
    private void Awake()
    {
        itemSlotIndex = 0; // Initialize itemSlotIndex to the first slot
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

    /// <summary>
    /// Initializes player input handler and focuses initial slot.
    /// </summary>
    private void Start()
    {
        InitializePlayerInput();
        FocusSlot(itemSlotIndex);
    }

    /// <summary>
    /// Handles hotbar navigation and input processing.
    /// </summary>
    private void Update()
    {
        if (m_playerInputHandler == null)
        {
            InitializePlayerInput();
            return;
        }

        HandleItemSwitching();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Uses the item in the currently selected slot.
    /// </summary>
    public void UseItem() => m_itemHotbar.UseItem(itemSlotIndex);

    /// <summary>
    /// Checks if the currently selected slot is empty.
    /// </summary>
    /// <returns>True if slot is empty, false if it contains an item</returns>
    public bool IsSlotEmpty()
    {
        var item = m_itemHotbar.GetItemAt(itemSlotIndex);
        return item.Amount <= 0;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initializes the player input handler reference.
    /// </summary>
    private void InitializePlayerInput()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[GameHotbar] Player not found in scene! Make sure player object exists with 'Player' tag.");
            return;
        }

        m_playerInputHandler = player.GetComponent<PlayerInputHandler>();
        if (m_playerInputHandler == null)
        {
            Debug.LogError("[GameHotbar] PlayerInputHandler component not found on player object!");
            return;
        }
    }

    /// <summary>
    /// Handles item slot switching based on player input.
    /// Supports cycling through slots in both directions.
    /// </summary>
    private void HandleItemSwitching()
    {
        var itemSwitchLeftInput = m_playerInputHandler.ItemSwitchLeftInput;
        var itemSwitchRightInput = m_playerInputHandler.ItemSwitchRightInput;

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

    /// <summary>
    /// Updates UI to show which slot is currently selected.
    /// </summary>
    /// <param name="index">Index of the slot to focus</param>
    private void FocusSlot(int index)
    {
        if (itemViewSlotDictionary.ContainsKey(index))
        {
            ItemViewSlot itemViewSlot = itemViewSlotDictionary[index];
            itemViewSlot.Select();
        }
    }
    #endregion
}
