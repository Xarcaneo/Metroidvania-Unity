using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Menu
{
    public abstract class Menu<T> : Menu where T : Menu<T>
    {
        private static T _instance;
        public static T Instance { get { return _instance; } }

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = (T)this;
            }
        }

        protected virtual void OnDestroy()
        {
            _instance = null;
        }

        public static void Open()
        {
            if (MenuManager.Instance != null && Instance != null)
            {
                MenuManager.Instance.OpenMenu(Instance);
            }
        }

    }

    [RequireComponent(typeof(Canvas))]
    public abstract class Menu : MonoBehaviour
    {
        [SerializeField] public GameObject buttonToFocus;

        protected Canvas canvas;

        private void Update()
        {
            if(canvas.worldCamera == null) canvas.worldCamera = Camera.main;

            CustomUpdate();
        }

        public virtual void OnStart() 
        {
            SetCanvas();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

        private void OnEnable()
        {
            InputManager.Instance.isInputActive = true;

            InputManager.Instance.OnMenuReturn += OnReturnInput;
            InputManager.Instance.OnMenuPlayerMenu += OnPlayerMenuInput;
            InputManager.Instance.OnMenuDelete += OnPlayerDeleteInput;
        }

        private void OnDisable()
        {
            InputManager.Instance.OnMenuReturn -= OnReturnInput;
            InputManager.Instance.OnMenuPlayerMenu -= OnPlayerMenuInput;
            InputManager.Instance.OnMenuDelete -= OnPlayerDeleteInput;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => SetCanvas();


        public virtual void SetCanvas()
        {
            canvas = gameObject.GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
        }

        public virtual void OnReturnInput() => OnBackPressed();
        public virtual void OnPlayerMenuInput() { }
        public virtual void OnPlayerDeleteInput() { }
        public virtual void OnOpenMenu() { }
        public virtual void CustomUpdate() { }

        public virtual void OnBackPressed()
        {
            if (MenuManager.Instance != null)
            {
                MenuManager.Instance.CloseMenu();
            }
        }
    }
}