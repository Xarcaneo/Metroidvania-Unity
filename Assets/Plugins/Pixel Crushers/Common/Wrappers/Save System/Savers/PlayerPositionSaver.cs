using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace PixelCrushers
{
    public class PlayerPositionSaver : PositionSaver
    {
        public override void ApplyData(string s)
        {
            base.ApplyData(s);
            var cameraPos = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
            Camera.main.transform.position = cameraPos;
            var vcam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            vcam.PreviousStateIsValid = false;
        }

    }
}