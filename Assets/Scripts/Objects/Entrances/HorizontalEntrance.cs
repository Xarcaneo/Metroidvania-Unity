using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalEntrance : Entrance
{
    public override void EntranceEntered()
    {
        base.EntranceEntered();

        InputManager.Instance.isInputActive = false;

        GameEvents.Instance.DeactivatePlayerInput(true);

        if (transform.localScale.x == -1)
            GameManager.Instance.shouldFlipPlayer = true;
    }
}
