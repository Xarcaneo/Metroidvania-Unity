using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    CinemachineVirtualCameraBase vcam;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCameraBase>();

        if (vcam != null && Player.Instance != null)
            vcam.LookAt = vcam.Follow = Player.Instance.transform;
    }
}
