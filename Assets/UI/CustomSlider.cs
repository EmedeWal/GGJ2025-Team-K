using UnityEngine;

public class CustomSlider : MonoBehaviour
{
    [Header("SETTINGS")]


    private float _minValue = 0;
    private float _maxValue = 1;
    private float _value = 1;

    private Controller _controller;

    void Start()
    {
        _controller = FindFirstObjectByType<Controller>();
        _maxValue = _controller.Attributes.GetCurrentHealth();
        _value = _maxValue;
    }

    void Update()
    {
        
    }

    void AddValue(float value)
    {
        _value += value;
        if (_value > _maxValue)
        {
            _value = _maxValue;
        }
    }

    void RemoveValue(float value)
    {
        _value -= value;
    }
}
