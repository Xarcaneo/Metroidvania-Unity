using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabButton : MonoBehaviour
{
    [SerializeField] private TabGroup tabGroup;

    public Image background;

    private void Awake()
    {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }
}
