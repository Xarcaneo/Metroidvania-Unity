using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveSlot : MyButton
{
    [SerializeField] private int SlotNumber;

    [Header("Content")]
    [SerializeField] private GameObject NoDataContent;
    [SerializeField] private GameObject HasDataContent;
    [SerializeField] private TextMeshProUGUI LocationText;
    [SerializeField] private TextMeshProUGUI SlotText;

    private void OnEnable()
    {
        SetButtonContent();
    }

    public void SetButtonContent()
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

    protected override void OnPressed()
    {
        if (Menu.PlayMenu.Instance.currentMode == Menu.PlayMenu.MenuMode.SelectLoad)
        {
            base.OnPressed();
        }
    }

    protected override void OnSelectAction()
    {
        base.OnSelectAction();

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

