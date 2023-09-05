using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalEntrance : Entrance
{
    [SerializeField] private bool entranceTop = false;

    public override void Start()
    {
        base.Start();

        if (entranceTop)
        {
            GameEvents.Instance.onPlayerStateChanged += onPlayerStateChanged;

            scenePortal.gameObject.SetActive(false);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (entranceTop)
        {
            GameEvents.Instance.onPlayerStateChanged -= onPlayerStateChanged;
        }
    }

    public override void EntranceEntered()
    {
        base.EntranceEntered();

        InputManager.Instance.isInputActive = false;

        GameEvents.Instance.DeactivatePlayerInput(true);
    }

    private void onPlayerStateChanged(State state)
    {
        if (state == Player.Instance.LadderClimbState)
            scenePortal.gameObject.SetActive(true);
        else
            scenePortal.gameObject.SetActive(false);
    }
}
