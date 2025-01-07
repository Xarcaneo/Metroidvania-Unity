using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    /// <summary>
    /// Represents the credits screen, displaying information about the game's creators.
    /// </summary>
    public class CreditsScreen : Menu<CreditsScreen>
    {
        /// <summary>
        /// Called when the credits screen is enabled.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            // Add any future initialization logic here.
        }

        /// <summary>
        /// Called when the credits screen is disabled.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            // Add any future cleanup logic here.
        }
    }
}
