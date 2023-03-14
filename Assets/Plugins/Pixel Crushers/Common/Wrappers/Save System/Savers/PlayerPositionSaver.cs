using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

namespace PixelCrushers
{
    public class PlayerPositionSaver : PositionSaver
    {
        public override void ApplyData(string s)
        {
            base.ApplyData(s);
            var cameraPos = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
            Camera.main.transform.position = cameraPos;

            ProCamera2D m_proCamera2D = FindObjectOfType<ProCamera2D>();
            m_proCamera2D.MoveCameraInstantlyToPosition(cameraPos);
        }

    }
}