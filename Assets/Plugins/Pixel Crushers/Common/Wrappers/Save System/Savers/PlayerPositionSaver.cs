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
        }

    }
}