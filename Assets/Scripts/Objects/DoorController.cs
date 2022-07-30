using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Player player;

    public Transform exitPoint;

    [SerializeField] private TransitionFader startTransitionPrefab;
    [SerializeField] private int levelToLoad;

    private void Start()
    {
        player = Player.Instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            StartCoroutine(UseDoorCorutine());
        }     
    }

    IEnumerator UseDoorCorutine()
    {
        TransitionFader.PlayTransition(startTransitionPrefab);
        player.enabled = false;

        yield return new WaitForSeconds(0.5f);

        player.transform.position = exitPoint.transform.position;

        Game.LevelLoader.Instance.LoadLevelAsync(levelToLoad);

        player.enabled = true;
    }
}
