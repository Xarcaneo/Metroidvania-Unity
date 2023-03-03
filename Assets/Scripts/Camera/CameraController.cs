using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    CinemachineVirtualCameraBase virtualCamera;
    bool isCameraInitialized = false;

    void Awake() =>
        // Subscribe to the player spawned event
        GameEvents.Instance.onPlayerSpawned += OnPlayerSpawned;

    void OnDestroy()
    {
        // Unsubscribe from the player spawned event
        GameEvents.Instance.onPlayerSpawned -= OnPlayerSpawned;
    }

    void OnPlayerSpawned()
    {
        if (!isCameraInitialized) // Initialize the camera only once
        {
            virtualCamera = GetComponent<CinemachineVirtualCameraBase>();

            if (virtualCamera != null && Player.Instance != null)
                virtualCamera.LookAt = virtualCamera.Follow = Player.Instance.transform;

            isCameraInitialized = true;
        }
    }

    void LateUpdate()
    {
        // Get the current camera position and round it to the nearest pixel
        Vector3 currentPosition = transform.position;
        Vector3 roundedPosition = new Vector3(Mathf.Round(currentPosition.x), Mathf.Round(currentPosition.y), currentPosition.z);

        // Set the camera position to the rounded position
        transform.position = roundedPosition;
    }
}
