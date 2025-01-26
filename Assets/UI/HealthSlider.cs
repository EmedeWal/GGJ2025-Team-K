using UnityEngine.UI;
using UnityEngine;

public class HealthSlider : MonoBehaviour
{
    [Header("SETTINGS")]

    [Space]
    [Header("Backdrop Fill")]
    [SerializeField] private Image _backdropFill;
    


    private Slider _slider;
    private float _targetValue;
    private bool _initialized;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.wholeNumbers = true;

        _targetValue = 0;
        _initialized = false;

        Attributes.HealthUpdated += HealthSlider_HealthUpdated;
    }

    private void LateUpdate()
    {
        if (_initialized)
            _slider.value = _targetValue;
    }

    private void HealthSlider_HealthUpdated(float value)
    {
        if (!_initialized)
        {
            _initialized = true;

            _slider.maxValue = value;
            _slider.minValue = 0;
            _slider.value = value;
        }
        _targetValue = value;
    }
}