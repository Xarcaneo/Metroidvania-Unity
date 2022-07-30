using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    void Start()
    {
        var vcam = GetComponent<CinemachineVirtualCameraBase>();
        if (vcam != null)
            vcam.LookAt = vcam.Follow = Player.Instance.transform;
    }
        
}
