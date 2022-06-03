using System.Collections;
using System.Collections.Generic;
using UnityCore.Menu;
using UnityEngine;

namespace UnityCore
{
        namespace Scene
        {
            public class ChangeSceneButton : MonoBehaviour
            {
                [SerializeField] private SceneController sceneController;

                [SerializeField] private SceneType sceneToOpen;

                public void Load()
                {
                    sceneController.Load(sceneToOpen, (_scene) => { Debug.Log("Scene [" + _scene + "] loaded from test script!"); }, false, PageType.Loading);
                }
            }
        }
    }