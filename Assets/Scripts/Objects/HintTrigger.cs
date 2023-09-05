using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    [SerializeField] private string hintID = "#000";
    [SerializeField] private HintBox m_hintBox;

    private void Start()
    {
        m_hintBox.SetHintText(hintID);
        m_hintBox.HideHintBox();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        m_hintBox.ShowHintBox();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        m_hintBox.HideHintBox();
    }
}
