using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    /// <summary>
    /// Generic base class for menu implementations that follows the singleton pattern.
    /// Provides basic menu functionality and ensures only one instance exists.
    /// </summary>
    /// <typeparam name="T">The specific menu type that inherits from this class</typeparam>
    public abstract class Menu<T> : Menu where T : Menu<T>
    {
        /// <summary>
        /// The single instance of this menu type
        /// </summary>
        private static T _instance;

        /// <summary>
        /// Public accessor for the menu instance
        /// </summary>
        public static T Instance { get { return _instance; } }

        /// <summary>
        /// Initializes the singleton instance. Destroys any duplicate instances.
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Debug.LogWarning($"Multiple instances of {typeof(T).Name} detected. Destroying duplicate.");
                Destroy(gameObject);
            }
            else
            {
                _instance = (T)this;
            }
        }

        /// <summary>
        /// Cleans up the singleton instance when the object is destroyed
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        /// <summary>
        /// Opens this menu through the MenuManager if both instances exist
        /// </summary>
        public static void Open()
        {
            if (MenuManager.Instance == null)
            {
                Debug.LogError($"Cannot open {typeof(T).Name}: MenuManager instance is missing!");
                return;
            }

            if (Instance == null)
            {
                Debug.LogError($"Cannot open {typeof(T).Name}: Menu instance is missing!");
                return;
            }

            MenuManager.Instance.OpenMenu(Instance);
        }
    }

    /// <summary>
    /// Base class for all menu behaviors. Handles input management and basic menu functionality.
    /// Requires a Canvas component to function.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public abstract class Menu : MonoBehaviour
    {
        /// <summary>
        /// Reference to the UI button that should receive focus when the menu opens
        /// </summary>
        [SerializeField]
        [Tooltip("The UI button that should receive focus when this menu opens")]
        public GameObject buttonToFocus;

        /// <summary>
        /// Reference to the menu's canvas component
        /// </summary>
        protected Canvas canvas;

        /// <summary>
        /// Cached reference to the main camera
        /// </summary>
        protected Camera mainCamera;

        /// <summary>
        /// Updates the canvas camera reference and performs custom update logic
        /// </summary>
        private void Update()
        {
            if (canvas == null)
            {
                Debug.LogError($"Canvas is missing on {gameObject.name}!");
                return;
            }

            // Only update camera reference if needed
            if (canvas.worldCamera == null)
            {
                if (mainCamera == null)
                    mainCamera = Camera.main;

                if (mainCamera == null)
                {
                    Debug.LogWarning("Main camera not found!");
                    return;
                }

                canvas.worldCamera = mainCamera;
            }

            CustomUpdate();
        }

        /// <summary>
        /// Initializes the menu and subscribes to scene loading events
        /// </summary>
        public virtual void OnStart()
        {
            if (!TryGetComponent(out canvas))
            {
                Debug.LogError($"Canvas component missing on {gameObject.name}!");
                return;
            }

            SetCanvas();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Cleans up scene loading event subscription
        /// </summary>
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Sets up input handling when the menu becomes active
        /// </summary>
        protected virtual void OnEnable()
        {
            if (InputManager.Instance == null)
            {
                Debug.LogError("InputManager instance is missing!");
                return;
            }

            InputManager.Instance.isInputActive = true;

            // Subscribe to input events
            InputManager.Instance.OnMenuReturn += OnReturnInput;
            InputManager.Instance.OnMenuPlayerMenu += OnPlayerMenuInput;
            InputManager.Instance.OnMenuDelete += OnPlayerDeleteInput;
        }

        /// <summary>
        /// Cleans up input handling when the menu becomes inactive
        /// </summary>
        protected virtual void OnDisable()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnMenuReturn -= OnReturnInput;
                InputManager.Instance.OnMenuPlayerMenu -= OnPlayerMenuInput;
                InputManager.Instance.OnMenuDelete -= OnPlayerDeleteInput;
            }
        }

        /// <summary>
        /// Handles scene loading events by resetting the canvas
        /// </summary>
        public virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode) => SetCanvas();

        /// <summary>
        /// Configures the canvas and its camera reference
        /// </summary>
        public virtual void SetCanvas()
        {
            if (MenuManager.Instance == null)
            {
                Debug.LogError("MenuManager instance is missing!");
                return;
            }

            if (MenuManager.Instance.m_camera == null)
            {
                Debug.LogError("MenuManager camera reference is missing!");
                return;
            }

            canvas.worldCamera = MenuManager.Instance.m_camera;
        }

        // Input handling virtual methods
        public virtual void OnReturnInput() => OnBackPressed();
        public virtual void OnPlayerMenuInput() { }
        public virtual void OnPlayerDeleteInput() { }

        /// <summary>
        /// Called when the menu is opened
        /// </summary>
        public virtual void OnOpenMenu()
        {
            if (buttonToFocus != null)
            {
                var button = buttonToFocus.GetComponent<Button>();
                if (button != null)
                {
                    button.Select();
                }
            }
        }

        /// <summary>
        /// Custom update logic to be implemented by derived classes
        /// </summary>
        public virtual void CustomUpdate() { }

        /// <summary>
        /// Handles the back button press, typically closing the current menu
        /// </summary>
        public virtual void OnBackPressed()
        {
            if (MenuManager.Instance == null)
            {
                Debug.LogError("Cannot close menu: MenuManager instance is missing!");
                return;
            }

            MenuManager.Instance.CloseMenu();
        }
    }
}
