using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelCrushers
{
    public class PlayerPositionSaver : PositionSaver
    {
        public bool isCheckpoint;
        static string previousCheckpointSceneName;
        static Vector3 previousCheckpointPosition;

        public override void ApplyData(string s)
        {
            if (usePlayerSpawnpoint && SaveSystem.playerSpawnpoint != null)
            {
                SetPosition(SaveSystem.playerSpawnpoint.transform.position, SaveSystem.playerSpawnpoint.transform.rotation);
            }
            else if (!string.IsNullOrEmpty(s))
            {
                var data = SaveSystem.Deserialize<PositionData>(s, m_data);
                if (data == null) return;

                m_data = data;
                previousCheckpointSceneName = m_data.checkpointSceneName;
                previousCheckpointPosition = m_data.position;

                SetPosition(data.position, data.rotation);          
            }

            var cameraPos = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
            Camera.main.transform.position = cameraPos;

            ProCamera2D m_proCamera2D = FindObjectOfType<ProCamera2D>();
            m_proCamera2D.MoveCameraInstantlyToPosition(cameraPos);
        }

        public override string RecordData()
        {
           var currentScene = SceneManager.GetActiveScene().buildIndex;

            if (isCheckpoint)
            {
                m_data.checkpointSceneName = SceneManager.GetActiveScene().name;
                SaveSystem.currentSavedGameData.sceneName = SceneManager.GetActiveScene().name;
                m_data.position = target.transform.position;

                previousCheckpointSceneName = m_data.checkpointSceneName;
                previousCheckpointPosition = m_data.position;
            }
            else
            {
                m_data.position = previousCheckpointPosition;
                m_data.checkpointSceneName = previousCheckpointSceneName;
                SaveSystem.currentSavedGameData.sceneName = previousCheckpointSceneName;
            }

            isCheckpoint = false;

            m_data.scene = currentScene;
            m_data.rotation = target.transform.rotation;
            return SaveSystem.Serialize(m_data);         
        }
    }
}