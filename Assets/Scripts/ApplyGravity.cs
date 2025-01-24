using UnityEditor.Search;
using UnityEngine;

public class ApplyGravity : MonoBehaviour
{
    [SerializeField] private float _gravity = 9.8f;
    private float _weight;

    private Rigidbody2D _rigidbody;

    private bool _isInBubble = false;
    public bool IsInBubble
    {
        get => _isInBubble;
        set => _isInBubble = value;
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _weight = _rigidbody.mass;
    }

    void Update()
    {
        if (!_isInBubble)
            AddGravity();
    }
    
    void AddGravity()
    {
        _rigidbody.AddForce(Vector2.down * _gravity * _weight);
    }
}
