using UnityEngine;

public class Button : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private GameObject _pressedObject;
    [SerializeField] private bool _isPressed = false;
    void Start()
    {

    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D()
    {
        if (!_isPressed)
        {
            _isPressed = true;
            var pos = _pressedObject.transform.position;
            pos.y -= 0.17f;
            _pressedObject.transform.position = pos;
        }
    }

    void OnTriggerExit2D()
    {
        if (_isPressed)
        {
            _isPressed = false;
            var pos = _pressedObject.transform.position;
            pos.y += 0.17f;
            _pressedObject.transform.position = pos;
        }
    }
}
