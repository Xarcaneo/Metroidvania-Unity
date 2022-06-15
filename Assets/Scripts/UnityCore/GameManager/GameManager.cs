using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        // reference to player
        private Player _player;

        private bool _tansitionIsOn;
        public bool IsTransitionOver { get { return _tansitionIsOn; } }

        [SerializeField]
        private string nextLevelName;

        [SerializeField]
        private int nextLevelIndex;

        private static GameManager _instance;

        public static GameManager Instance { get => _instance; }

        // initialize references
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }

            _player = Object.FindObjectOfType<Player>();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        // end the level
        public void ExitLevel()
        {
/*            if (_player != null)
            {
                // disable the player controls
                ThirdPersonUserControl thirdPersonControl =
                    _player.GetComponent<ThirdPersonUserControl>();

                if (thirdPersonControl != null)
                {
                    thirdPersonControl.enabled = false;
                }

                // remove any existing motion on the player
                Rigidbody rbody = _player.GetComponent<Rigidbody>();
                if (rbody != null)
                {
                    rbody.velocity = Vector3.zero;
                }

                // force the player to a stand still
                _player.Move(Vector3.zero, false, false);
            }*/

            // check if we have set IsGameOver to true, only run this logic once
            LoadLevel(nextLevelName);

        }

        public void LoadLevel(string levelName)
        {
            SceneManager.LoadScene(levelName);
        }

        public void LoadLevel(int levelIndex)
        {
            SceneManager.LoadScene(levelIndex);
        }

        // check for the end game condition on each frame
        private void Update()
        {
            if (_tansitionIsOn)
            {
                ExitLevel();
            }
        }
    }
}