using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    [SerializeField] private string hintID = "#000";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Menu.GameMenu.Instance.hintBox.SetHintText(hintID);
        Menu.GameMenu.Instance.hintBox.FadeIn();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Menu.GameMenu.Instance.hintBox.FadeOut();
    }
}
