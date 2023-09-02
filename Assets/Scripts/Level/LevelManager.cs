using QFSW.QC;
using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Player m_playerPref;
    [SerializeField] private string m_areaName;

    private void OnEnable()
    {
        SpawnPlayer();

        Menu.GameMenu.Instance.locationNameIndicator.Cancelcoroutine();

        if (m_areaName != "")
            StartCoroutine(DelayedAreaChanged());

        QuantumConsole.Instance.canToggleConsole = true;
    }

    private void OnDisable()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
            GameObject.Destroy(item);

        QuantumConsole.Instance.canToggleConsole = false;
        QuantumConsole.Instance.Deactivate();
    }
    private IEnumerator DelayedAreaChanged()
    {
        yield return new WaitForSeconds(0.5f);
        GameEvents.Instance.AreaChanged(m_areaName);
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
            Instantiate(m_playerPref, new Vector3(-10, -10, 0), Quaternion.identity);
        }
    }
}
