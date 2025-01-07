using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages level-specific functionality including player spawning and scene transitions.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private Player m_playerPref;
    [SerializeField] private Vector3 m_defaultSpawnPosition = new Vector3(-10, -10, 0);

    [Header("Level Settings")]
    [SerializeField] private string m_areaName;

    private void OnEnable()
    {
        SpawnPlayer();

        Menu.GameMenu.Instance.locationNameIndicator.Cancelcoroutine();

        if (m_areaName != "")
            StartCoroutine(DelayedAreaChanged());
    }

    private void OnDisable()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
            GameObject.Destroy(item);
    }

    /// <summary>
    /// Spawns the player in the level, either at a designated spawn point or at the default position.
    /// Handles player orientation based on the entrance used to enter the level.
    /// </summary>
    private void SpawnPlayer()
    {
        GameObject spawnPointObj = GameObject.FindGameObjectWithTag("SpawnPoint");
        if (spawnPointObj != null)
        {
            // Spawn player at the designated spawn point
            var position = spawnPointObj.transform.position;
            var player = Instantiate(m_playerPref, position, Quaternion.identity);

            // Set initial facing direction based on spawn point
            SetPlayerFacingDirection(player.gameObject, spawnPointObj);

            // Clean up the spawn point
            CleanupSpawnPoint(spawnPointObj);
        }
        else
        {
            // No spawn point found, use default position
            Instantiate(m_playerPref, m_defaultSpawnPosition, Quaternion.identity);
        }
    }

    /// <summary>
    /// Sets the player's facing direction based on the spawn point's configuration.
    /// </summary>
    /// <param name="player">The spawned player instance</param>
    /// <param name="spawnPointObj">The spawn point object containing direction information</param>
    private void SetPlayerFacingDirection(GameObject player, GameObject spawnPointObj)
    {
        if (player == null) return;

        var spawnPoint = spawnPointObj.GetComponent<SpawnPoint>();
        if (spawnPoint == null) return;

        var playerComponent = player.GetComponent<Player>();
        if (playerComponent == null) return;

        var movement = playerComponent.Core.GetCoreComponent<Movement>();
        if (movement != null)
        {
            // shouldFaceLeft true -> face left (-1), false -> face right (1)
            int direction = spawnPoint.shouldFaceLeft ? -1 : 1;
            movement.Flip(direction);
        }
    }

    /// <summary>
    /// Cleans up the spawn point object after player spawning.
    /// Ensures proper cleanup when using DontDestroyOnLoad.
    /// </summary>
    /// <param name="spawnPointObj">The spawn point object to clean up</param>
    private void CleanupSpawnPoint(GameObject spawnPointObj)
    {
        if (spawnPointObj == null) return;

        // Move back to current scene before destroying to prevent memory leaks
        SceneManager.MoveGameObjectToScene(spawnPointObj, SceneManager.GetActiveScene());
        Destroy(spawnPointObj);
    }

    /// <summary>
    /// Triggers the area changed event after a short delay.
    /// </summary>
    private IEnumerator DelayedAreaChanged()
    {
        yield return new WaitForSeconds(0.5f);
        GameEvents.Instance.AreaChanged(m_areaName);
    }
}
