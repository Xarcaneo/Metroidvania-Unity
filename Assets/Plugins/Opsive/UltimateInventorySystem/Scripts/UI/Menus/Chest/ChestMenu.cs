﻿/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Menus.Chest
{
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Item;
    using Opsive.UltimateInventorySystem.UI.Panels;
    using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
    using System.Threading.Tasks;
    using Opsive.UltimateInventorySystem.UI.Currency;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    /// <summary>
    /// The Chest menu is used to display and retrieve items from a chest. 
    /// </summary>
    public class ChestMenu : InventoryPanelBinding
    {
        [Tooltip("The inventory Grid.")]
        [SerializeField] protected internal InventoryGrid m_InventoryGrid;
        [Tooltip("The Multi Currency View showing the chest currency. (Optional)")]
        [SerializeField] protected internal MultiCurrencyView m_MultiCurrencyView;
        [Tooltip("The take all button.")]
        [SerializeField] protected Button m_TakeCurrencyButton;
        [Tooltip("The take all button.")]
        [SerializeField] protected Button m_TakeAllItemsButton;
        [Tooltip("The take all button.")]
        [SerializeField] protected Button m_TakeAllButton;
        [Tooltip("The cancel button.")]
        [SerializeField] protected Button m_CancelButton;
        [Tooltip("The Quantity picker panel.")]
        [SerializeField] protected internal QuantityPickerPanel m_QuantityPickerPanel;
        [Tooltip("Do not close the chest if the chest inventory is empty.")]
        [SerializeField] protected bool m_DoNotCloseChestIfEmpty;
        [Tooltip("ItemCollection To add the pickedup chest items to")]
        [SerializeField] protected string m_AddToItemCollectionName;

        protected IChest m_Chest;

        public InventoryGrid InventoryGrid => m_InventoryGrid;

        /// <summary>
        /// Setup the component.
        /// </summary>
        public override void Initialize(DisplayPanel display, bool force)
        {
            var wasInitialized = m_IsInitialized;
            if (wasInitialized && !force) { return; }
            base.Initialize(display, force);

            if (wasInitialized == false) {
                //Do it only once even if forced.
                
                if (m_TakeCurrencyButton != null) {
                    m_TakeCurrencyButton.onClick.AddListener(TakeCurrency);
                }

                if (m_TakeAllItemsButton != null) {
                    m_TakeAllItemsButton.onClick.AddListener(TakeAllItems);
                }
                
                if (m_TakeAllButton != null) {
                    m_TakeAllButton.onClick.AddListener(TakeAll);
                }

                if (m_CancelButton != null) {
                    m_CancelButton.onClick.AddListener(CancelButtonClicked);
                }
                
            }

            m_InventoryGrid.Initialize(force);
            m_InventoryGrid.SetDisplayPanel(m_DisplayPanel);
            m_InventoryGrid.OnItemViewSlotClicked -= ItemClicked;
            m_InventoryGrid.OnItemViewSlotClicked += ItemClicked;

            FindAndBindChests();
        }

        /// <summary>
        /// Handle the cancel button being clicked.
        /// </summary>
        protected virtual void CancelButtonClicked()
        {
            m_DisplayPanel.Close();
        }

        /// <summary>
        /// Handle the inventory being bound.
        /// </summary>
        protected override void OnInventoryBound()
        { }

        /// <summary>
        /// Find and reference all the chests in the scene.
        /// </summary>
        protected virtual void FindAndBindChests()
        {
#if UNITY_2023_1_OR_NEWER
            var allChests = FindObjectsByType<Chest>(FindObjectsSortMode.None);
#else
            var allChests = FindObjectsOfType<Chest>();
#endif
            
            for (int i = 0; i < allChests.Length; i++) {
                if(allChests[i].ChestMenu != null){ continue; }
                allChests[i].ChestMenu = this;
            }
        }

        /// <summary>
        /// Take all the items from the chest.
        /// </summary>
        protected virtual void TakeAll()
        {
            TakeCurrency();
            TakeAllItems();
        }
        
        /// <summary>
        /// Take all the items from the chest.
        /// </summary>
        protected virtual void TakeAllItems()
        {
            var clientItemCollection = m_Inventory.GetItemCollection(m_AddToItemCollectionName);
            if (clientItemCollection == null) {
                clientItemCollection = m_Inventory.MainItemCollection;
            }
            
            m_Chest.Inventory.MainItemCollection.GiveAllItems(
                clientItemCollection, null);
            
            m_DisplayPanel.Close();
        }
        
        /// <summary>
        /// Take all the items from the chest.
        /// </summary>
        protected virtual void TakeCurrency()
        {
            var chestCurrencyOwner = m_Chest.Inventory.GetCurrencyOwner();
            if(chestCurrencyOwner == null){ return; }

            var playerCurrencyOwner = m_Inventory.GetCurrencyOwner();
            if(playerCurrencyOwner == null){ return; }

            playerCurrencyOwner.CurrencyAmount.AddCurrency(chestCurrencyOwner.CurrencyAmount);
            chestCurrencyOwner.CurrencyAmount.RemoveAll();
            
            if (m_MultiCurrencyView != null) {
                m_MultiCurrencyView.DrawCurrency(chestCurrencyOwner.CurrencyAmount);
            }
        }

        /// <summary>
        /// Handle the On Open event.
        /// </summary>
        public override void OnOpen()
        {
            base.OnOpen();

            m_QuantityPickerPanel.Close(false);

            m_InventoryGrid.SetInventory(m_Chest.Inventory);
            m_InventoryGrid.Draw();
            m_InventoryGrid.Grid.SelectButton(0);
            
            if (m_MultiCurrencyView != null) {
                m_MultiCurrencyView.DrawCurrency(m_Chest.Inventory.GetCurrencyOwner()?.CurrencyAmount);
            }
        }

        /// <summary>
        /// Set the chest.
        /// </summary>
        /// <param name="chest">The chest.</param>
        public virtual void SetChest(IChest chest)
        {
            m_Chest = chest;
        }

        /// <summary>
        /// Click an item amount.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        /// <param name="index">The index.</param>
        protected virtual void ItemClicked(ItemViewSlotEventData slotEventData)
        {
            var itemInfo = slotEventData.ItemViewSlot.ItemInfo;
            m_QuantityPickerPanel.Open(m_DisplayPanel, slotEventData.ItemViewSlot);

            m_QuantityPickerPanel.QuantityPicker.MinQuantity = 0;
            m_QuantityPickerPanel.QuantityPicker.MaxQuantity = itemInfo.Amount;

            m_QuantityPickerPanel.ConfirmCancelPanel.SetConfirmText("Take");
            m_QuantityPickerPanel.QuantityPicker.SetQuantity(itemInfo.Amount);

#pragma warning disable 4014
            WaitForQuantityDecision(itemInfo.Item);
#pragma warning restore 4014
        }

        /// <summary>
        /// Wait for the quantity picker to return an amount.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The task to await.</returns>
        protected virtual async Task WaitForQuantityDecision(Item item)
        {
            var quantity = await m_QuantityPickerPanel.WaitForQuantity();

            if (quantity < 1) { return; }

            var itemInfo = (ItemInfo)(item, quantity);
            
            
            var clientItemCollection = m_Inventory.GetItemCollection(m_AddToItemCollectionName);
            if (clientItemCollection == null) {
                clientItemCollection = m_Inventory.MainItemCollection;
            }
            m_Chest.Inventory.MainItemCollection.GiveItem(
                itemInfo,
                clientItemCollection,
                null);

            m_InventoryGrid.Draw();
        }

        /// <summary>
        /// Close the UI panel.
        /// </summary>
        public override void OnClose()
        {
            m_QuantityPickerPanel.Close(false);
            
            base.OnClose();

            if (m_DoNotCloseChestIfEmpty && m_Chest.Inventory.MainItemCollection.GetAllItemStacks().Count == 0) {
                return;
            }
            
            m_Chest.Close();
        }
    }
}
