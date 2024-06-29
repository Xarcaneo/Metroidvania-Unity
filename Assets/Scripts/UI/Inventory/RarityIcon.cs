using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RarityIcon : MonoBehaviour
{
    [SerializeField] private Sprite[] rarityImages;
    [SerializeField] private string[] rarityNames;
    [SerializeField] private Image rarityIcon;
    [SerializeField] private TextMeshProUGUI text;
    public void SetValue(ItemRarity rarity)
    {
        rarityIcon.enabled = true;
        rarityIcon.sprite = rarityImages[(int)rarity];
        text.text = rarityNames[(int)rarity];
    }

    public void Clear()
    {
        rarityIcon.enabled = false;
        text.text = "";
    }
}
