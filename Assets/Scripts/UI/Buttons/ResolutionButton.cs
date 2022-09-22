using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionButton : MyButton
{
    public event Action<int> Pressed;
    public string text = "";
    public int buttonIndex;

    private void Start()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    protected override void OnPressedAction()
    {
        base.OnPressedAction();

        Pressed?.Invoke(buttonIndex);
    }
}
