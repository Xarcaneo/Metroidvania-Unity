using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapTab : Tab
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private WorldMapManager m_worldMapManager;

    [SerializeField] private float moveSpeed = 400f;

    private void OnEnable()
    {
        m_worldMapManager.gameObject.SetActive(true);
        UpdateRectPosition();
    }

    private void OnDisable()
    {
        m_worldMapManager.gameObject.SetActive(false);
    }

    private void Update()
    {
        scrollRect.content.position -= new Vector3(CalculateRectMove().x, CalculateRectMove().y, 0);
    }

    private Vector2 CalculateRectMove()
    {
        return InputManager.Instance.GetNavigateValue() * moveSpeed * Time.unscaledDeltaTime;
    }

    private void UpdateRectPosition()
    {
        if (m_worldMapManager.m_activeRoom)
        {
            scrollRect.FocusOnItem(m_worldMapManager.m_activeRoom.GetComponent<RectTransform>());
        }
    }
}
