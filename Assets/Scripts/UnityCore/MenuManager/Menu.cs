using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        private PlayerInput playerInput;
        public int ReturnInput { get; private set; }

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
        }

        public virtual void OnOpenMenu()
        {
        }

        public virtual void OnReturnInput(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                OnBackPressed();
            }
        }

        public virtual void OnBackPressed()
        {
            if (MenuManager.Instance != null)
            {
                MenuManager.Instance.CloseMenu();
            }
        }
    }
}