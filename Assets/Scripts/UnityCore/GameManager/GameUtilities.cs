using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameUtilities : MonoBehaviour
    {
        [SerializeField] GameObject eventPrefab;

        private void Awake()
        {
            if (GameObject.Find("EventSystem(Clone)") == null)
            {
                Instantiate(eventPrefab);
            }
        }
    }
}