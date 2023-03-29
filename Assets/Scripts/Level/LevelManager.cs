using Audio;
using PixelCrushers;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] int ChapterNumber = 0;
    [SerializeField] int m_roomID = 0;

    [SerializeField] Player m_playerPref;

    private void OnEnable()
    {
        AudioManager.Instance.Jukebox.SetAudioCollection(ChapterNumber);
        AudioManager.Instance.Jukebox.gameObject.SetActive(true);
        GameEvents.Instance.LevelLoaded(m_roomID);

        SpawnPlayer();
    }

    private void OnDisable()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
            GameObject.Destroy(item);
    }

    private void SpawnPlayer()
    {
        if (GameObject.FindGameObjectWithTag("SpawnPoint"))
        {
            var position = GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;
            Instantiate(m_playerPref, position, Quaternion.identity);
        }
        else
        {
            Instantiate(m_playerPref, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
