using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveSlot : MonoBehaviour, ISelectHandler
{
    [SerializeField] private int SlotNumber;
    [SerializeField] private GameObject NoDataContent;
    [SerializeField] private GameObject HasDataContent;

    public void Update()
    {
        bool is_empty = SaveSystem.HasSavedGameInSlot(SlotNumber);

        if (is_empty)
        {
            HasDataContent.SetActive(true);
            NoDataContent.SetActive(false);

        }
        else
        {
            HasDataContent.SetActive(false);
            NoDataContent.SetActive(true);
        }
    }

    public void DeleteSave(int slot)
    {
        SaveSystem.DeleteSavedGameInSlot(slot);
    }

    public void OnSelect(BaseEventData eventData)
    {
        GameManager.Instance.currentSaveSlot = SlotNumber;
    }
}
