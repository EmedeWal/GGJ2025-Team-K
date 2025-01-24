using UnityEngine;

public class Bubble : MonoBehaviour, IKillable
{
    private GameObject _capturedObject;
    public GameObject CapturedObject
    {
        get => _capturedObject;
        set => _capturedObject = value;
    }

    [Range(0, 1)]
    [SerializeField] private float _transparency;

    private bool _isCaptured = false;

    [Header("SETTINGS")]

    [Space]
    [SerializeField] private float _initialForce = 30f;
    [SerializeField] private float _response = 3f;
    [SerializeField] private float _lifeTime = 3f;
    [SerializeField] private float _intensity = 3f;
    [SerializeField] private float _frequency = 3f;

    private Rigidbody2D _rigidbody;
    private Transform _transform;
    private float _timer = 0;

    private float _force;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _transform = transform;
        
        Utils.SetRigidbody(_rigidbody);

        _force = _initialForce;
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;
        
        if (!_isCaptured)
        {
            _timer += deltaTime;
            if (_timer > _lifeTime)
                Debug.Log("lifetime over");
        }


        // Change this by clicking later
        if (Input.GetKeyDown(KeyCode.E))
            OnRelease();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        //Calculate sine wave oscillation along the "up" direction
        var wave = Mathf.Sin(_timer * _frequency) * _rigidbody.linearVelocity.magnitude * _intensity;
        var up = new Vector2(_transform.up.x, _transform.up.y);
        _rigidbody.linearVelocity = up * wave;

        var response = 1f - Mathf.Exp(-_response * Time.fixedDeltaTime);
        _force = Mathf.Lerp(_force, 0, response);
        _rigidbody.AddForce(_transform.right * _force, ForceMode2D.Impulse);

        if (_rigidbody.linearVelocity.magnitude < 0.05f)
            _rigidbody.linearVelocity = Vector2.zero;
    }

    public void OnSpikeHit() => OnRelease();

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Capturable"))
        {
            _capturedObject = collision.gameObject;
            OnCapture();
        }
    }

    void OnCapture()
    {
        ChangeTransparency(_transparency);

        _capturedObject.transform.SetParent(transform);
        _capturedObject.transform.localPosition = Vector2.zero;
        _capturedObject.GetComponent<Collider2D>().enabled = false;
    }

    void OnRelease()
    {
        ChangeTransparency(1);
        _capturedObject.GetComponent<Collider2D>().enabled = true;
        _capturedObject.transform.SetParent(null);

        Destroy(gameObject);
    }

    void ChangeTransparency(float newTransparency)
    {
        var renderer = _capturedObject.GetComponentInChildren<SpriteRenderer>();
        var colour = renderer.color;
        colour.a = newTransparency;
        renderer.color = colour;
    }
}
