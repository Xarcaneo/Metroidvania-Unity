using Com.LuisPedroFonseca.ProCamera2D;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraController : MonoBehaviour
{
    private ProCamera2D m_proCamera2D;

    [HideInInspector] public List<GameObject> targets = new List<GameObject>();

    void Awake()
    {
        // Find all objects with the CameraTarget script and add them to the targets list
        CameraHook[] cameraTargets = FindObjectsOfType<CameraHook>();
        foreach (CameraHook target in cameraTargets)
        {
            targets.Add(target.gameObject);
        }

        // Subscribe to the player spawned event
        GameEvents.Instance.onPlayerSpawned += OnPlayerSpawned;
        GameEvents.Instance.onCameraNewTarget += OnCameraNewTarget;

        m_proCamera2D = GetComponent<ProCamera2D>();
    }

    void OnDestroy()
    {
        // Unsubscribe from the player spawned event
        GameEvents.Instance.onPlayerSpawned -= OnPlayerSpawned;
        GameEvents.Instance.onCameraNewTarget -= OnCameraNewTarget;
    }

    private void OnCameraNewTarget(string hookID)
    {
        // Find the CameraHook object with the specified ID
        CameraHook target = FindObjectsOfType<CameraHook>().FirstOrDefault(t => t.HookID == hookID);

        m_proCamera2D.RemoveAllCameraTargets();
        m_proCamera2D.AddCameraTarget(target.transform);
    }

    void OnPlayerSpawned()
    {
        m_proCamera2D.AddCameraTarget(Player.Instance.transform);
        m_proCamera2D.CenterOnTargets();
    }
}
