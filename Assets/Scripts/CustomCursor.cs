using UnityEngine.UI;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [Header("SETTINGS")]

    [Space]
    [Header("Color")]
    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _chargeColor = Color.yellow;
    [SerializeField] private Color _hoverColor = Color.green;

    private RectTransform _rectTransform;
    private Image _image;

    private void Start()
    {
        Cursor.visible = false;

        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.anchoredPosition = Vector2.zero;

        _image = GetComponent<Image>();
        _image.color = _defaultColor;
    }

    public void UpdateColorAndPosition(bool hit, bool charging)
    {
        var color = charging
            ? _chargeColor
            : hit
            ? _hoverColor
            : _defaultColor;

        _image.color = color;
        _rectTransform.position = Input.mousePosition;
    }
}
