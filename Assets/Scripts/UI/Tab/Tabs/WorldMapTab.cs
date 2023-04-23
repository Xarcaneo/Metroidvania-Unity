using Menu;
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

    private WorldMapManager _worldMapManagerInstance;

    public void Initialize()
    {
        GameEvents.Instance.onNewSession += OnNewSession;
        GameEvents.Instance.onEndSession += OnEndSession;
    }
    private void OnDestroy()
    {
        GameEvents.Instance.onNewSession -= OnNewSession;
        GameEvents.Instance.onEndSession -= OnEndSession;
    }

    private void OnEnable()
    {
        if (_worldMapManagerInstance != null)
        {
            if(_worldMapManagerInstance.transform != transform)
                _worldMapManagerInstance.transform.SetParent(transform);

            _worldMapManagerInstance.gameObject.SetActive(true);
        }

        UpdateRectPosition();
    }

    private void OnDisable()
    {
        if (_worldMapManagerInstance != null)
            _worldMapManagerInstance.gameObject.SetActive(false);
    }

    private void OnNewSession()
    {
        _worldMapManagerInstance = Instantiate(m_worldMapManager);
        scrollRect.content = _worldMapManagerInstance.GetComponent<RectTransform>();
        _worldMapManagerInstance.Initialize();
    }

    private void OnEndSession()
    {
        if (_worldMapManagerInstance != null)
        {
            _worldMapManagerInstance.Deinitialize();
            Destroy(_worldMapManagerInstance.gameObject);
            _worldMapManagerInstance = null;
        }
    }

    private void Update()
    {
        if (_worldMapManagerInstance != null)
            scrollRect.content.position -= new Vector3(CalculateRectMove().x, CalculateRectMove().y, 0);
    }

    private Vector2 CalculateRectMove()
    {
        return InputManager.Instance.GetNavigateValue() * moveSpeed * Time.unscaledDeltaTime;
    }

    private void UpdateRectPosition()
    {
        if (_worldMapManagerInstance != null)
            if (_worldMapManagerInstance.m_activeRoom)
            {
                scrollRect.FocusOnItem(_worldMapManagerInstance.m_activeRoom.GetComponent<RectTransform>());
            }
    }
}
