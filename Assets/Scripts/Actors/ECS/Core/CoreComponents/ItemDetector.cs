using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDetector : CoreComponent
{
    public event Action<bool> onItemDetected;

    private int itemsDetected = 0;

    protected override void Awake()
    {
        base.Awake();

        try
        {
            GameEvents.Instance.onPauseTrigger += EnableDisableComponent;
            GameEvents.Instance.onDialogueTrigger += EnableDisableComponent;
        }
        catch
        {
        }
    }

    private void OnDestroy()
    {
        try
        {
            GameEvents.Instance.onPauseTrigger -= EnableDisableComponent;
            GameEvents.Instance.onDialogueTrigger -= EnableDisableComponent;
        }
        catch
        {
        }
    }

    public override void EnableDisableComponent(bool state)
    {
        base.EnableDisableComponent(!state);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        itemsDetected++;
        onItemDetected?.Invoke(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        itemsDetected--;

        if (itemsDetected == 0)
        {
            onItemDetected?.Invoke(false);
        }
    }
}
