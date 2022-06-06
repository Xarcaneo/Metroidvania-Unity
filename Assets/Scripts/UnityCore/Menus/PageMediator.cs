using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityCore
{
    namespace Menu
    {
        public class PageMediator : MonoBehaviour
        {
            [SerializeField] PageType pageToOpen;
            [SerializeField] PageType pageToClose;

            private PageController m_Menu;

            // get menu with integrity
            private PageController menu
            {
                get
                {
                    if (m_Menu == null)
                    {
                        m_Menu = PageController.instance;
                    }
                    if (m_Menu == null)
                    {

                    }
                    return m_Menu;
                }
            }

            public void TurnPageOn() => menu.TurnPageOn(pageToOpen);
            public void TurnPageOff( bool _waitForExit ) => menu.TurnPageOff(pageToClose, pageToOpen, _waitForExit);
        }
    }
}
