using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveSlot : MyButton, ISelectHandler
{
    [SerializeField] private int SlotNumber;

    [Header("Content")]
    [SerializeField] private GameObject NoDataContent;
    [SerializeField] private GameObject HasDataContent;
    [SerializeField] private TextMeshProUGUI LocationText;
    [SerializeField] private TextMeshProUGUI SlotText;

    private void OnEnable()
    {
        bool is_empty = SaveSystem.HasSavedGameInSlot(SlotNumber);

        if (is_empty)
        {
            UpdateContent();
            HasDataContent.SetActive(true);
            NoDataContent.SetActive(false);

        }
        else
        {
            HasDataContent.SetActive(false);
            NoDataContent.SetActive(true);
        }
    }

    protected override void OnSelectAction()
    {
        base.OnPressedAction();

        GameManager.Instance.currentSaveSlot = SlotNumber;
    }

    private void UpdateContent()
    {
        var savedGameData = SaveSystem.storer.RetrieveSavedGameData(SlotNumber);
        var s = savedGameData.GetData("GameSummary");
        var summary = SaveSystem.Deserialize<GameSummarySaver.Data>(s);

        LocationText.text = "Location: " + summary.sceneName;
        SlotText.text = "Slot: " + SlotNumber;
        
    }
}

