using Menu;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the world map tab in the UI, handling map display, navigation, and room tracking.
/// </summary>
public class WorldMapTab : Tab
{
    [Header("References")]
    [Tooltip("ScrollRect component for map navigation")]
    [SerializeField] private ScrollRect scrollRect;
    [Tooltip("Prefab for the world map manager")]
    [SerializeField] private WorldMapManager m_worldMapManager;

    [Header("Navigation Settings")]
    [Tooltip("Speed at which the map can be navigated")]
    [SerializeField] private float moveSpeed = 400f;

    private WorldMapManager _worldMapManagerInstance;

    /// <summary>
    /// Initializes the world map tab and subscribes to game session events.
    /// </summary>
    public void Initialize()
    {
        GameEvents.Instance.onNewSession += OnNewSession;
        GameEvents.Instance.onEndSession += OnEndSession;
    }

    /// <summary>
    /// Cleans up event subscriptions when the tab is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        GameEvents.Instance.onNewSession -= OnNewSession;
        GameEvents.Instance.onEndSession -= OnEndSession;
    }

    /// <summary>
    /// Sets up the map display when the tab is enabled.
    /// </summary>
    private void OnEnable()
    {
        if (_worldMapManagerInstance != null)
        {
            if(_worldMapManagerInstance.transform != transform)
                _worldMapManagerInstance.transform.SetParent(transform);

            _worldMapManagerInstance.gameObject.SetActive(true);
            _worldMapManagerInstance.transform.SetSiblingIndex(0);
        }

        UpdateRectPosition();
    }

    /// <summary>
    /// Hides the map display when the tab is disabled.
    /// </summary>
    private void OnDisable()
    {
        if (_worldMapManagerInstance != null)
            _worldMapManagerInstance.gameObject.SetActive(false);
    }

    /// <summary>
    /// Handles the creation and initialization of the world map when a new game session starts.
    /// </summary>
    private void OnNewSession()
    {
        if (_worldMapManagerInstance == null)
        {
            _worldMapManagerInstance = Instantiate(m_worldMapManager);
            scrollRect.content = _worldMapManagerInstance.GetComponent<RectTransform>();
            _worldMapManagerInstance.Initialize();
        }
    }

    /// <summary>
    /// Cleans up the world map when a game session ends.
    /// </summary>
    private void OnEndSession()
    {
        if (_worldMapManagerInstance != null)
        {
            _worldMapManagerInstance.Deinitialize();
            Destroy(_worldMapManagerInstance.gameObject);
            _worldMapManagerInstance = null;
        }
    }

    /// <summary>
    /// Updates the map position based on player input.
    /// </summary>
    private void Update()
    {
        if (_worldMapManagerInstance != null)
            scrollRect.content.position -= new Vector3(CalculateRectMove().x, CalculateRectMove().y, 0);
    }

    /// <summary>
    /// Calculates the movement of the map based on input.
    /// </summary>
    /// <returns>The movement vector for the map scroll position.</returns>
    private Vector2 CalculateRectMove()
    {
        return InputManager.Instance.GetNavigateValue() * moveSpeed * Time.unscaledDeltaTime;
    }

    /// <summary>
    /// Centers the map view on the currently active room.
    /// </summary>
    private void UpdateRectPosition()
    {
        if (_worldMapManagerInstance != null && _worldMapManagerInstance.m_activeRoom)
        {
            scrollRect.FocusOnItem(_worldMapManagerInstance.m_activeRoom.GetComponent<RectTransform>());
        }
    }
}
