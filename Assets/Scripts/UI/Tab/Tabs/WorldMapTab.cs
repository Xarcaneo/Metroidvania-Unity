using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapTab : Tab
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float moveSpeed = 400f;

    private void Update()
    {
        scrollRect.content.anchoredPosition += CalculateRectMove();
    }

    private Vector2 CalculateRectMove()
    {
        return InputManager.Instance.GetNavigateValue() * moveSpeed * Time.unscaledDeltaTime;
    }
}
