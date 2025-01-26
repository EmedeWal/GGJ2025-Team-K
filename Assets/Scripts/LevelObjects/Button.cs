using UnityEngine;
using System.Collections.Generic;
using System;

public class Button : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private GameObject _pressedObject;
    [SerializeField] private bool _isPressed = false;
    [SerializeField] private bool _playerCanPress = false;

    [SerializeField] private List<TriggerableByButton> triggers;

    private void Start()
    {
        LevelManager.ResetGameState += LevelManager_resetGameState;
    }

    private void LevelManager_resetGameState()
    {
        if (_isPressed)
        {
            _isPressed = false;
            var pos = _pressedObject.transform.position;
            pos.y += 0.17f;
            _pressedObject.transform.position = pos;
        }
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
            foreach (var x in triggers)
            {
                x._onButtonPress();
            }
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
            foreach (var x in triggers)
            {
                x._onButtonRelease();
            }
        }
    }
}
