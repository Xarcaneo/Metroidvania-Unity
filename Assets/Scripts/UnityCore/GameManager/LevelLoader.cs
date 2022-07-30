using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Menu;

namespace Game
{
    public class LevelLoader : MonoBehaviour
    {
        private bool _tansitionIsOn;
        public bool IsTransitionOver { get { return _tansitionIsOn; } }

        [SerializeField] private string nextLevelName;
        [SerializeField] private int nextLevelIndex;

        private static LevelLoader _instance;

        public static LevelLoader Instance { get => _instance; }

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
            SceneManager.LoadSceneAsync(levelName);
        }

        public void LoadLevelAsync(int levelIndex)
        {
            StartCoroutine(LoadLevelAsyncRoutine(levelIndex));
        }

        private static IEnumerator LoadLevelAsyncRoutine(int levelIndex)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelIndex);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            yield return null;
            asyncLoad.allowSceneActivation = true;
        }
    }
}