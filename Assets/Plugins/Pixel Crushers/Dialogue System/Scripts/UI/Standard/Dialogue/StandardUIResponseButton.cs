using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace PixelCrushers.DialogueSystem
{
    [AddComponentMenu("")] // Use wrapper.
    public class StandardUIResponseButton : MonoBehaviour, ISelectHandler
    {
        [Header("Button & UI Elements")]
        public UnityEngine.UI.Button button;
        public UITextField label;
        public bool setLabelColor = true;
        public Color defaultColor = Color.white;

        public Response response { get; set; }
        public Transform target { get; set; }
        public bool isVisible { get; set; }
        public bool isClickable
        {
            get { return (button != null) && button.interactable; }
            set { if (button != null) button.interactable = value; }
        }

        [Header("New Input System (Optional)")]
        [Tooltip("If assigned, pressing this action will call OnClick() just like a normal button click.")]
        public InputActionReference inputActionRef;

        // If you want, you can specify a toggle to enable/disable input checking:
        [Tooltip("Whether to listen for inputActionRef events.")]
        public bool listenForInputAction = true;

        public virtual string text
        {
            get { return label.text; }
            set
            {
                label.text = UITools.StripRPGMakerCodes(value);
                UITools.SendTextChangeMessage(label);
            }
        }

        public virtual void Awake()
        {
            if (button == null) button = GetComponent<UnityEngine.UI.Button>();
            if (button == null)
            {
                Debug.LogWarning("Dialogue System: Response button '" + name + "' is missing a Unity UI Button component!", this);
            }
        }

        public virtual void Start()
        {
            // Auto-assign OnClick() if button has no persistent listeners:
            if (button != null && button.onClick.GetPersistentEventCount() == 0)
            {
                button.onClick.AddListener(OnClick);
            }
        }

        private void OnEnable()
        {
            // Enable your input action (if assigned and not null):
            if (inputActionRef != null && inputActionRef.action != null)
            {
                inputActionRef.action.performed += OnActionPerformed;
            }
        }

        private void OnDisable()
        {
            // Disable your input action to avoid memory leaks / redundant callbacks:
            if (inputActionRef != null && inputActionRef.action != null)
            {
                inputActionRef.action.performed -= OnActionPerformed;
            }
        }

        // Callback from the new Input System
        private void OnActionPerformed(InputAction.CallbackContext ctx)
        {
            if (!listenForInputAction) return;

            if (EventSystem.current != null &&
                EventSystem.current.currentSelectedGameObject == gameObject &&
                isClickable && isVisible)
            {
                OnClick();
            }
        }

        public virtual void Reset()
        {
            isClickable = false;
            isVisible = false;
            response = null;
            if (label != null)
            {
                label.text = string.Empty;
                SetColor(defaultColor);
            }
        }

        /// <summary>
        /// Call this when you want to programmatically "click" the button.
        /// </summary>
        public virtual void OnClick()
        {
            if (target != null)
            {
                SetCurrentResponse();
                target.SendMessage("OnClick", response, SendMessageOptions.RequireReceiver);
            }
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            SetCurrentResponse();
        }

        protected virtual void SetCurrentResponse()
        {
            if (DialogueManager.instance != null &&
                DialogueManager.instance.conversationController != null)
            {
                DialogueManager.instance.conversationController.SetCurrentResponse(response);
            }
        }

        public virtual void SetFormattedText(FormattedText formattedText)
        {
            if (formattedText == null) return;
            text = UITools.GetUIFormattedText(formattedText);
            SetColor((formattedText.emphases.Length > 0) ? formattedText.emphases[0].color : defaultColor);
        }

        public virtual void SetUnformattedText(string unformattedText)
        {
            text = unformattedText;
            SetColor(defaultColor);
        }

        protected virtual void SetColor(Color currentColor)
        {
            if (setLabelColor && label != null)
            {
                label.color = currentColor;
            }
        }
    }
}
