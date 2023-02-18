using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionButton : MyButton
{
    public event Action<int> Pressed;
    public int buttonIndex;

    protected override void OnPressedAction()
    {
        base.OnPressedAction();

        Pressed?.Invoke(buttonIndex);
    }
}
