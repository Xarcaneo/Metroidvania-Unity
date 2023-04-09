using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ButtonFocusDetector : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color deselectedColor;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private bool isFirstButton;

    private void OnEnable() => textMeshProUGUI.color = isFirstButton ? selectedColor : deselectedColor;

    private void OnDisable() => textMeshProUGUI.color = deselectedColor;

    public void OnSelect(BaseEventData eventData) => textMeshProUGUI.color = selectedColor;

    public void OnDeselect(BaseEventData eventData) => textMeshProUGUI.color = deselectedColor;
}
