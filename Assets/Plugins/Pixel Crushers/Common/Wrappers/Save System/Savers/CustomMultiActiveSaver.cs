using PixelCrushers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomMultiActiveSaver : MultiActiveSaver
{
    public override void Awake()
    {
        base.Awake();

        m_key = "OnlyOnLoad_" + SceneManager.GetActiveScene().name;

        // Find all gameObjects with the "SpawnableEnemy" tag and add them to the gameObjectsToWatch array:
        var spawnableEnemies = GameObject.FindGameObjectsWithTag("SpawnableEnemy");
        gameObjectsToWatch = new GameObject[spawnableEnemies.Length];
        for (int i = 0; i < spawnableEnemies.Length; i++)
        {
            gameObjectsToWatch[i] = spawnableEnemies[i];
        }
    }

    public override void ApplyData(string s)
    {
        if (!CustomMultiActiveSaverManager.IsLoadingSavedGame)
        {
            base.ApplyData(s);
        }
    }
}
