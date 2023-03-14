using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    private ProCamera2D m_proCamera2D;

    void Awake()
    {
        // Subscribe to the player spawned event
        GameEvents.Instance.onPlayerSpawned += OnPlayerSpawned;

        m_proCamera2D = GetComponent<ProCamera2D>();
    }

    void OnDestroy()
    {
        // Unsubscribe from the player spawned event
        GameEvents.Instance.onPlayerSpawned -= OnPlayerSpawned;
    }

    void OnPlayerSpawned()
    {
        m_proCamera2D.AddCameraTarget(Player.Instance.transform);
        m_proCamera2D.CenterOnTargets();
    }
}
