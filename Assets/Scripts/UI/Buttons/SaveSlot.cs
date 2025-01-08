using PixelCrushers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Button that represents a save game slot in the menu.
/// Handles displaying save data and managing slot interactions.
/// </summary>
public class SaveSlot : MyButton
{
    #region Serialized Fields
    /// <summary>
    /// The save slot number this button represents.
    /// </summary>
    [SerializeField] private int SlotNumber;

    [Header("Content")]
    /// <summary>
    /// UI elements shown when slot has no save data.
    /// </summary>
    [SerializeField] private GameObject NoDataContent;

    /// <summary>
    /// UI elements shown when slot has save data.
    /// </summary>
    [SerializeField] private GameObject HasDataContent;

    /// <summary>
    /// Text showing the location/level name from save data.
    /// </summary>
    [SerializeField] private TextMeshProUGUI LocationText;

    /// <summary>
    /// Text showing the slot number or other slot info.
    /// </summary>
    [SerializeField] private TextMeshProUGUI SlotText;
    #endregion

    #region Unity Event Functions
    /// <summary>
    /// Updates button content when enabled.
    /// </summary>
    private void OnEnable()
    {
        SetButtonContent();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Updates the button's visual state based on save data.
    /// Shows appropriate content for empty/filled slots.
    /// </summary>
    public void SetButtonContent()
    {
        bool has_save_data = SaveSystem.HasSavedGameInSlot(SlotNumber);

        if (has_save_data)
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
    #endregion

    #region Protected Methods
    /// <summary>
    /// Only allows button press in load mode.
    /// Prevents accidental saves/loads.
    /// </summary>
    protected override void OnPressed()
    {
        if (Menu.PlayMenu.Instance.currentMode == Menu.PlayMenu.MenuMode.SelectLoad)
        {
            base.OnPressed();
        }
    }

    /// <summary>
    /// Updates PlayMenu with this slot's number when selected.
    /// </summary>
    protected override void OnSelectAction()
    {
        base.OnSelectAction();
        GameManager.Instance.currentSaveSlot = SlotNumber;  
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Updates the button's text content with save data.
    /// Called when slot has save data.
    /// </summary>
    private void UpdateContent()
    {
        var savedGameData = SaveSystem.storer.RetrieveSavedGameData(SlotNumber);
        var s = savedGameData.GetData("GameSummary");
        var summary = SaveSystem.Deserialize<GameSummarySaver.Data>(s);

        LocationText.text = "Location: " + summary.sceneName;
        SlotText.text = "Slot: " + SlotNumber;
    }
    #endregion
}
