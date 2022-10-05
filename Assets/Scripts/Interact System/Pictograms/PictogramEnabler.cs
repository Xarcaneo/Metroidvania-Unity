using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictogramEnabler : MonoBehaviour
{
    [SerializeField]
    private GameObject pictogram = null;

    internal new bool enabled
    {
        get => pictogram.activeSelf;
        set
        {
            if (enabled == value) return;

            pictogram.SetActive(value);
        }
    }

    private void Awake()
    {
        enabled = false;
    }
}
