using UnityEngine;

public class Button : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private GameObject _pressedObject;
    [SerializeField] private bool _isPressed = false;
    [SerializeField] private bool _playerCanPress = false;
    void Start()
    {

    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<Controller>(out var controller) && !_playerCanPress)
        {
            return;
        }
        if (!_isPressed)
        {
            _isPressed = true;
            var pos = _pressedObject.transform.position;
            pos.y -= 0.17f;
            _pressedObject.transform.position = pos;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent<Controller>(out var controller) && !_playerCanPress)
        {
            return;
        }

        if (_isPressed)
        {
            _isPressed = false;
            var pos = _pressedObject.transform.position;
            pos.y += 0.17f;
            _pressedObject.transform.position = pos;
        }
    }
}
