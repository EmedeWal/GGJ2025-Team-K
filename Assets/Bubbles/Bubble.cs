using UnityEngine;

public class Bubble : MonoBehaviour, IKillable
{
    [Header("SETTINGS")]

    [Space]
    [SerializeField] private float _lifeTime = 3f;
    [SerializeField] private float _intensity = 3f;
    [SerializeField] private float _frequency = 3f;

    private Rigidbody2D _rigidbody;
    private Transform _transform;
    private float _timer = 0;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _transform = transform;
        
        Utils.SetRigidbody(_rigidbody);
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        HandleUp();

        if (_timer > _lifeTime)
        {
            Debug.Log("lifetime over");
        }
    }

    private void HandleForward()
    {

    }

    private void HandleUp()
    {
        var up = new Vector2(_transform.up.x, _transform.up.y);
        var intensity = Mathf.Sin(_timer * _frequency);
        var upVelocity = _intensity * intensity * up;

        _rigidbody.linearVelocity = upVelocity;
    }

    public void OnSpikeHit() { }
}
