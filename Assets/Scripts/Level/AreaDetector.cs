using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDetector : MonoBehaviour
{
    [SerializeField] int roomNumber;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameEvents.Instance.RoomChanged(roomNumber);
    }
}
