using UnityEngine;

namespace GameServices
{
    /// <summary>
    /// Bootstrapper class responsible for initializing essential game services.
    /// </summary>
    public static class Bootstrapper
    {
        /// <summary>
        /// Method to perform initial setup before the first scene is loaded.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Setup()
        {
            // Initialize the default service locator
            ServiceRegistry.Initialize();

            // Register services here
            // Example: ServiceRegistry.Instance.RegisterService<IUIManager>(new UIManager());
        }
    }
}
