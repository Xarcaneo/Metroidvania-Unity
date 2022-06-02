using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityCore
{
    namespace Menu
    {
        public class ChangePageButton : MonoBehaviour
        {
            [SerializeField] private PageController pageController;

            [SerializeField] private PageType pageToOpen;
            [SerializeField] private PageType pageToClose;

            public void TurnPageOn()
            {
                pageController.TurnPageOn(pageToOpen);
            }
            public void TurnPageOff()
            {
                pageController.TurnPageOff(pageToClose);
            }

            public void SwitchPage()
            {
                pageController.TurnPageOff(pageToClose, pageToOpen);
            }
        }
    }
}