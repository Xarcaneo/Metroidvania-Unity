using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace PixelCrushers
{

    public class PlayerPositionSaver : Saver
    {
        [Tooltip("Player last checkpoint position.")]
        public Vector3 m_lastCheckpointPosition = new Vector3(0,0,0);

        [Tooltip("Player last checkpoint scene.")]
        public int m_lastCheckpointScene = 0;

        [Tooltip("If set, save position of target. Otherwise save this GameObject's position.")]
        [SerializeField]
        private Transform m_target = null;

        [Tooltip("When changing scenes, if a player spawnpoint is specified, move this GameObject to the spawnpoint.")]
        [SerializeField]
        private bool m_usePlayerSpawnpoint = false;

        [Tooltip("Record positions in every scene. If unticked, only records position in most recent scene.")]
        [SerializeField]
        private bool m_multiscene = false;

        [Serializable]
        public class PositionData
        {
            public string checkpointSceneName;
            public int scene = -1;
            public Vector3 position;
            public Quaternion rotation;
        }

        [Serializable]
        public class ScenePositionData
        {
            public int scene;
            public Vector3 position;
            public Quaternion rotation;
            public ScenePositionData(int _scene, Vector3 _position, Quaternion _rotation)
            {
                scene = _scene;
                position = _position;
                rotation = _rotation;
            }
        }

        [Serializable]
        public class MultiscenePositionData
        {
            public List<ScenePositionData> positions = new List<ScenePositionData>();
        }

        public PositionData m_data;
        public bool isCheckpoint;
        public string previousCheckpointSceneName;
        protected MultiscenePositionData m_multisceneData;
        protected NavMeshAgent m_navMeshAgent;

        public Transform target
        {
            get { return (m_target == null) ? this.transform : m_target; }
            set { m_target = value; }
        }

        public bool usePlayerSpawnpoint
        {
            get { return m_usePlayerSpawnpoint; }
            set { m_usePlayerSpawnpoint = value; }
        }

        protected bool multiscene { get { return m_multiscene; } }

        public override void Awake()
        {
            base.Awake();
            if (m_multiscene) m_multisceneData = new MultiscenePositionData();
            else m_data = new PositionData();
            m_navMeshAgent = target.GetComponent<NavMeshAgent>();
        }

        public override string RecordData()
        {
            m_data.checkpointSceneName = isCheckpoint ? SceneManager.GetActiveScene().name : previousCheckpointSceneName;
            previousCheckpointSceneName = m_data.checkpointSceneName;
            m_data.position = m_lastCheckpointPosition;


            isCheckpoint = false;

            return SaveSystem.Serialize(m_data);     
        }

        public override void ApplyData(string s)
        {
            if (usePlayerSpawnpoint && SaveSystem.playerSpawnpoint != null)
            {
                SetPosition(SaveSystem.playerSpawnpoint.transform.position, SaveSystem.playerSpawnpoint.transform.rotation);
            }
            else if (!string.IsNullOrEmpty(s))
            {
                var currentScene = SceneManager.GetActiveScene().buildIndex;
                if (multiscene)
                {
                    var multisceneData = SaveSystem.Deserialize<MultiscenePositionData>(s, m_multisceneData);
                    if (multisceneData == null) return;
                    m_multisceneData = multisceneData;
                    for (int i = 0; i < m_multisceneData.positions.Count; i++)
                    {
                        if (m_multisceneData.positions[i].scene == currentScene)
                        {
                            SetPosition(m_multisceneData.positions[i].position, m_multisceneData.positions[i].rotation);
                            break;
                        }
                    }
                }
                else
                {
                    var data = SaveSystem.Deserialize<PositionData>(s, m_data);
                    if (data == null) return;
                    m_data = data;
                    if (data.scene == currentScene || data.scene == -1)
                    {
                        SetPosition(data.position, data.rotation);
                    }
                }
            }
        }

        protected virtual void SetPosition(Vector3 position, Quaternion rotation)
        {
            if (m_navMeshAgent != null)
            {
                m_navMeshAgent.Warp(position);
            }
            else
            {
                target.transform.position = position;
            }
            target.transform.rotation = rotation;
        }

    }
}