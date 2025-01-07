using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the display of item rarity through icons and text in the UI.
/// Handles the visualization of different rarity levels for inventory items.
/// </summary>
public class RarityIcon : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>
    /// Array of sprites representing different rarity levels.
    /// Order should match the ItemRarity enum order.
    /// </summary>
    [SerializeField] private Sprite[] rarityImages;

    /// <summary>
    /// Array of names for different rarity levels.
    /// Order should match the ItemRarity enum order.
    /// </summary>
    [SerializeField] private string[] rarityNames;

    /// <summary>
    /// Image component to display the rarity icon.
    /// </summary>
    [SerializeField] private Image rarityIcon;

    /// <summary>
    /// Text component to display the rarity name.
    /// </summary>
    [SerializeField] private TextMeshProUGUI text;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        ValidateComponents();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Sets the rarity icon and text based on the provided rarity level.
    /// </summary>
    /// <param name="rarity">The rarity level to display</param>
    public void SetValue(ItemRarity rarity)
    {
        if (!ValidateRarityArrays((int)rarity))
        {
            Clear();
            return;
        }

        rarityIcon.enabled = true;
        rarityIcon.sprite = rarityImages[(int)rarity];
        text.text = rarityNames[(int)rarity];
    }

    /// <summary>
    /// Clears the rarity display, hiding the icon and removing the text.
    /// </summary>
    public void Clear()
    {
        if (rarityIcon != null)
        {
            rarityIcon.enabled = false;
        }
        
        if (text != null)
        {
            text.text = string.Empty;
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Validates that all required components are properly assigned.
    /// </summary>
    private void ValidateComponents()
    {
        if (rarityIcon == null)
        {
            Debug.LogError($"[RarityIcon] Missing rarityIcon Image component on {gameObject.name}");
            enabled = false;
            return;
        }

        if (text == null)
        {
            Debug.LogError($"[RarityIcon] Missing text component on {gameObject.name}");
            enabled = false;
            return;
        }

        if (rarityImages == null || rarityImages.Length == 0)
        {
            Debug.LogError($"[RarityIcon] No rarity images assigned on {gameObject.name}");
            enabled = false;
            return;
        }

        if (rarityNames == null || rarityNames.Length == 0)
        {
            Debug.LogError($"[RarityIcon] No rarity names assigned on {gameObject.name}");
            enabled = false;
            return;
        }

        if (rarityImages.Length != rarityNames.Length)
        {
            Debug.LogError($"[RarityIcon] Mismatch between rarity images ({rarityImages.Length}) and names ({rarityNames.Length}) on {gameObject.name}");
            enabled = false;
        }
    }

    /// <summary>
    /// Validates that the rarity index is within the valid range of the arrays.
    /// </summary>
    /// <param name="rarityIndex">Index to validate</param>
    /// <returns>True if the index is valid, false otherwise</returns>
    private bool ValidateRarityArrays(int rarityIndex)
    {
        return rarityIndex >= 0 && 
               rarityIndex < rarityImages.Length && 
               rarityIndex < rarityNames.Length && 
               rarityImages[rarityIndex] != null;
    }
    #endregion
}
