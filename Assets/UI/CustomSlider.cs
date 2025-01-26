using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class CustomSlider : MonoBehaviour
{
    [Header("SETTINGS")]
    [Space]
    [Header("Prefabs")]
    [SerializeField] private GameObject _newSliderPrefab;

    private float _minValue = 0;
    private float _maxValue = 100;
    private float _value = 100;

    private Slider _thisSlider;
    private Slider _newSlider;
    private Controller _controller;

    private bool _isCharging = false;
    public bool IsCharging => _isCharging;
    

    void Start()
    {
        _thisSlider = GetComponent<Slider>();
        _controller = FindFirstObjectByType<Controller>();
        _thisSlider.maxValue = 100;
        _thisSlider.minValue = 0;
        _thisSlider.value = 100;

        LevelManager.ResetGameState += Slider_ResetGameState;
    }

    void FixedUpdate()
    {
        print(_thisSlider.value);
    }

    public void InitializeChargeSlider()
    {
        var tempSlider = Instantiate(_newSliderPrefab, gameObject.transform.parent.transform);
        tempSlider.transform.position = gameObject.transform.position;        
        _newSlider = tempSlider.GetComponent<Slider>();
        _newSlider.value = 10;
        _newSlider.maxValue = 20;
        _newSlider.minValue = 0;

        _isCharging = true;
    }
    public void SliderCharging(float charge)
    {
        _newSlider.value = 10 * charge;
    }

    public void EndCharge(float charge)
    {
        AddValue(-10 * charge);
        Destroy(_newSlider.gameObject);
        _isCharging = false;
    }

    public void AddValue(float v)
    {
        print("increased value by " + v);
        _value += v;
        _thisSlider.value = _value;
    }

    public void Slider_ResetGameState()
    {
        _value = 100;
        _isCharging = false;
        _thisSlider.value = _value;
    }
}
