using UnityEngine;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour
{
    [Header("SETTINGS")]
    [Space]
    [Header("Prefabs")]
    [SerializeField] private GameObject _newSliderPrefab;

    private float _minValue = 0;
    private float _maxValue = 100;
    private float _value = 1;

    private Slider _thisSlider;
    private Slider _newSlider;
    private Controller _controller;

    private bool _isCharging = false;
    public bool IsCharging { get; private set; }
    

    void Start()
    {
        _thisSlider = GetComponent<Slider>();
        _controller = FindFirstObjectByType<Controller>();
        _thisSlider.maxValue = 100;
        _thisSlider.minValue = 0;
        _thisSlider.value = _thisSlider.maxValue;
    }

    void FixedUpdate()
    {
        
    }

    public void InitializeChargeSlider()
    {
        var tempSlider = Instantiate(_newSliderPrefab, new Vector3(-(195 + (100 - _value)), 195.5f), Quaternion.identity);
        _newSlider = tempSlider.GetComponent<Slider>();
        _newSlider.value = 1;
        _newSlider.maxValue = 2;
        _newSlider.minValue = 0;

        _isCharging = true;
    }
    public void SliderCharging(float charge)
    {
        _newSlider.value = charge;
    }

    public void EndCharge(float charge)
    {
        _thisSlider.value -= charge;
        Destroy(_newSlider.gameObject);
        _isCharging = false;
    }


    // void AddValue(float value, Slider slider)
    // {
    //     _value += value;
    //     if (_value > _maxValue)
    //     {
    //         _value = _maxValue;
    //     }
    // }

    // void RemoveValue(float value, Slider slider)
    // {
    //     _value -= value;
    // }
}
