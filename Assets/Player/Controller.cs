using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Controller : MonoBehaviour, IKillable
{
    public Rigidbody2D Rigidbody => _rigidbody;

    [Header("SETTINGS")]

    [Space]
    [Header("Movement")]
    [SerializeField] private float _friction = 0.25f;
    [SerializeField] private float _acceleration = 16;
    [SerializeField] private float _deceleration = 13;
    [SerializeField] private float _movementSpeed = 9f;
    [SerializeField] private float _velocityPower = 0.95f;

    [Space]
    [Header("Jumping")]
    [Range(0f, 0.5f)]
    [SerializeField] private float _jumpBuffer = 0.1f;
    [Range(0f, 0.5f)]
    [SerializeField] private float _coyoteTime = 0.15f;
    [Range(0f, 1f)]
    [SerializeField] private float _airMultiplier = 0.4f;
    [Range(0f, 1f)]
    [SerializeField] private float _jumpCutMultiplier = 0.6f;
    [SerializeField] private float _jumpForce = 20f;

    [Space]
    [Header("Gravity")]
    [SerializeField] private float _defaultGravity = 3f;
    [SerializeField] private float _gravityIncrement = 0.03f;

    [Space]
    [Header("Other")]
    [SerializeField] private float _groundCheckSize = 0.2f;

    private BoxCollider2D _boxCollider;
    private Rigidbody2D _rigidbody;
    private LayerMask _groundLayers;

    private int _requestedMovement = 0;
    private float _timeSinceGrounded = 0;
    private float _timeSinceJumpRequest = 0;
    private bool _requestedJumpCut = false;

    private bool _locked = false;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _groundLayers = LayerMask.GetMask("Ground");

        Utils.SetRigidbody(_rigidbody);
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;

        TickTimers(deltaTime);
        UpdateInput();
    }

    private void FixedUpdate()
    {
        var grounded = PerformGroundCheck();

        if (grounded)
        {
            _timeSinceGrounded = _coyoteTime;
            _rigidbody.gravityScale = _defaultGravity;
        }

        HandleTurns();
        HorizontalMovement(grounded);
        VerticalMovement(grounded);
        HandleFriction(Mathf.Abs(_requestedMovement));
    }

    private void UpdateInput()
    {
        _requestedMovement = (int)Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
            _timeSinceJumpRequest = _jumpBuffer;

        _requestedJumpCut = _requestedJumpCut || Input.GetKeyUp(KeyCode.Space);
    }

    private void TickTimers(float deltaTime)
    {
        _timeSinceGrounded -= deltaTime;
        _timeSinceJumpRequest -= deltaTime;
    }

    private void HandleTurns()
    {
        var localScale = transform.localScale;
        if (localScale.x > 0 && _requestedMovement < 0 || localScale.x < 0 && _requestedMovement > 0)
        {
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    private void HorizontalMovement(bool grounded)
    {
        var targetSpeed = _requestedMovement * _movementSpeed;
        var speeddifference = targetSpeed - _rigidbody.linearVelocity.x;
        var accelerationRate = (Mathf.Abs(targetSpeed) > 0.01) ? _acceleration : _deceleration;

        if (!grounded)
            accelerationRate *= _airMultiplier;

        // No idea what this does but it works!
        var movementForce = Mathf.Pow(Mathf.Abs(speeddifference) * accelerationRate, _velocityPower) * Mathf.Sign(speeddifference);
        _rigidbody.AddForce(movementForce * Vector2.right, ForceMode2D.Force);
    }

    private void VerticalMovement(bool grounded)
    {
        if (_timeSinceGrounded > 0 && _timeSinceJumpRequest > 0)
            PerformJump();

        if (!grounded)
        {
            _rigidbody.gravityScale += _gravityIncrement;

            if (_requestedJumpCut && _rigidbody.linearVelocity.y > 0)
                _rigidbody.AddForce((1 - _jumpCutMultiplier) * _rigidbody.linearVelocity.y * Vector2.down, ForceMode2D.Impulse);
        }
        _requestedJumpCut = false;
    }

    private void PerformJump()
    {
        // Reset timers
        _timeSinceGrounded = 0;
        _timeSinceJumpRequest = 0;
        // Reset velocity, and gravity
        _rigidbody.gravityScale = _defaultGravity;
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0);

        // Apply force
        _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
    }

    private void HandleFriction(int inputMagnitude)
    {
        var horizontal = _rigidbody.linearVelocity.x;

        // If no input, apply friction
        if (inputMagnitude < 0.01f)
        {
            var friction = _friction * Mathf.Sign(horizontal);
            _rigidbody.AddForce(Vector2.right * -friction, ForceMode2D.Impulse);
        }

        // If almost still, stop player
        if (Mathf.Abs(horizontal) < 0.05)
            _rigidbody.linearVelocity = new Vector2(0, _rigidbody.linearVelocity.y);
    }

    private bool PerformGroundCheck()
    {
        var origin = _boxCollider.bounds.center;
        origin.y -= _boxCollider.bounds.extents.y;
        var size = new Vector2(_boxCollider.size.x * 0.8f, _groundCheckSize);
        return Physics2D.OverlapBox(origin, size, 0, _groundLayers);
    }

    public void Kill()
    {
        Debug.Log("Player death");
    }

    
    // For turning red on death or something, but perhaps playing a particle is better
    //private IEnumerator ColorCoroutine(Color color, float duration)
    //{
    //    _spriteRenderer.color = color;
    //    float time = 0;

    //    while (time < duration)
    //    {
    //        time += _deltaTime;

    //        float transitionTime = time / duration;
    //        _spriteRenderer.color = Color.Lerp(color, _originalColor, transitionTime);

    //        yield return null;
    //    }

    //    _spriteRenderer.color = _originalColor;
    //}
}