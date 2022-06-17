using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Menu;

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

        [SerializeField]
        private int mainMenuIndex = 0;

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

        public void LoadLevel(string levelName)
        {
            SceneManager.LoadScene(levelName);
        }

        public void LoadLevel(int levelIndex)
        { 
            SceneManager.LoadScene(levelIndex);
        }
    }
}