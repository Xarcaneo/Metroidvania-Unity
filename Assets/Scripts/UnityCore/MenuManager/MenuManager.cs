using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.EventSystems;

namespace Menu
{
    /// <summary>
    /// Manages the menus in the game, handling their initialization, opening, and closing.
    /// Ensures that only one menu is active at a time and maintains a stack of open menus.
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] public Camera m_camera;

        [Header("Menus prefabs")]
        [SerializeField] private MainMenu mainMenuPrefab;
        [SerializeField] private SettingsMenu settingsMenuPrefab;
        [SerializeField] private CreditsScreen creditsScreenPrefab;
        [SerializeField] private GameMenu gameMenuPrefab;
        [SerializeField] private PauseMenu pauseMenuPrefab;
        [SerializeField] private PlayMenu playMenuPrefab;
        [SerializeField] private GameSettingsMenu gameSettingsMenuPrefab;
        [SerializeField] private KeyboardControlsMenu keyboardControlsMenuPrefab;
        [SerializeField] private GamepadControlsMenu gamepadControlsMenuPrefab;
        [SerializeField] private GraphicSettingsMenu graphicSettingsMenu;
        [SerializeField] private ResolutionSettingsMenu resolutionSettingsMenu;
        [SerializeField] private AudioSettingsMenu audioSettingsMenu;
        [SerializeField] private LanguageSettingsMenu languageSettingsMenu;
        [SerializeField] private PlayerMenu playerMenu;
        [SerializeField] private DeathMenu deathMenuPrefab;

        [SerializeField]
        private Transform _menuParent;

        private Stack<Menu> _menuStack = new Stack<Menu>();

        private static MenuManager _instance;

        /// <summary>
        /// Singleton instance of the MenuManager.
        /// </summary>
        public static MenuManager Instance { get { return _instance; } }

        /// <summary>
        /// Called when the MenuManager is instantiated.
        /// Initializes the singleton instance and ensures it persists across scenes.
        /// </summary>
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                Object.DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// Called when the MenuManager starts.
        /// Initializes the menus if the instance is valid.
        /// </summary>
        private void Start()
        {
            if (_instance) InitializeMenus();
        }

        /// <summary>
        /// Called when the MenuManager is destroyed.
        /// Clears the singleton instance if it matches the current instance.
        /// </summary>
        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        /// <summary>
        /// Initializes all menu prefabs and sets the main menu as the initial active menu.
        /// </summary>
        private void InitializeMenus()
        {
            if (_menuParent == null)
            {
                GameObject menuParentObject = new GameObject("Menus");
                _menuParent = menuParentObject.transform;
            }

            DontDestroyOnLoad(_menuParent.gameObject);

            BindingFlags myFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            FieldInfo[] fields = this.GetType().GetFields(myFlags);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsSubclassOf(typeof(Menu)))
                {
                    Menu prefab = field.GetValue(this) as Menu;

                    if (prefab == null)
                    {
                        Debug.LogError($"MENUMANAGER InitializeMenus ERROR: {field.Name} prefab is missing!");
                        continue;
                    }

                    Menu menuInstance = Instantiate(prefab, _menuParent);
                    menuInstance.OnStart();

                    if (prefab != mainMenuPrefab)
                    {
                        menuInstance.gameObject.SetActive(false);
                    }
                    else
                    {
                        OpenMenu(menuInstance);
                    }
                }
            }
        }

        /// <summary>
        /// Opens the specified menu instance and pushes it onto the menu stack.
        /// </summary>
        /// <param name="menuInstance">The menu instance to open.</param>
        public void OpenMenu(Menu menuInstance)
        {
            if (menuInstance == null)
            {
                Debug.LogError("MENUMANAGER OpenMenu ERROR: invalid menu");
                return;
            }

            if (_menuStack.Count > 0)
            {
                foreach (Menu menu in _menuStack)
                {
                    menu.gameObject.SetActive(false);
                }
            }

            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(menuInstance.buttonToFocus, null);
            }
            else
            {
                Debug.LogError("EventSystem is not available.");
            }

            menuInstance.OnOpenMenu();
            menuInstance.gameObject.SetActive(true);
            _menuStack.Push(menuInstance);
        }

        /// <summary>
        /// Closes the top menu on the stack and activates the next menu if available.
        /// </summary>
        public void CloseMenu()
        {
            if (_menuStack.Count == 0)
            {
                Debug.LogWarning("MENUMANAGER CloseMenu ERROR: No menus in stack!");
                return;
            }

            Menu topMenu = _menuStack.Pop();
            topMenu.gameObject.SetActive(false);

            if (_menuStack.Count > 0)
            {
                Menu nextMenu = _menuStack.Peek();
                if (EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(nextMenu.buttonToFocus, null);
                }
                nextMenu.gameObject.SetActive(true);
            }
        }
    }
}
