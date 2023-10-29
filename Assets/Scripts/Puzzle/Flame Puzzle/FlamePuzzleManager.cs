using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlamePuzzleManager : MonoBehaviour
{
    private static FlamePuzzleManager _instance;

    public static FlamePuzzleManager Instance { get => _instance; }

    [SerializeField]
    private GameObject[] puzzlePrefabs;

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

    // Function to instantiate a GameObject in a specific scene based on ID
    public GameObject InstantiateObject(int puzzleID, string sceneName)
    {
        if (puzzleID >= 0 && puzzleID < puzzlePrefabs.Length)
        {
            GameObject prefabToInstantiate = puzzlePrefabs[puzzleID];
            Scene targetScene = SceneManager.GetSceneByName(sceneName);

            if (targetScene.IsValid())
            {
                GameObject instance = Instantiate(prefabToInstantiate, Vector3.zero, Quaternion.identity);
                SceneManager.MoveGameObjectToScene(instance, targetScene);
                SceneManager.SetActiveScene(targetScene);
                return instance;
            }
            else
            {
                Debug.LogError("Scene " + sceneName + " not found.");
            }
        }
        else
        {
            Debug.LogError("Invalid puzzle ID: " + puzzleID);
        }

        return null;
    }
}
