using System.Collections.Generic;
using UnityEngine;
using Bubbles;
using System;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Controller : MonoBehaviour, IKillable
{
    public Attributes Attributes => _attributes;
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
    [SerializeField] private float _defaultGravity = 2f;
    [SerializeField] private float _gravityIncrement = 0.02f;

    [Space]
    [Header("Other")]
    [SerializeField] private float _bubbleCheckSize = 0.5f;
    [SerializeField] private float _groundCheckSize = 0.2f;

    [Header("BUBBLES")]

    [Space]
    [Header("Shooting")]
    [SerializeField] private Bubble _bubblePrefab;
    [SerializeField] private float _spawnOffset = 2.4f;

    [Space]
    [Header("Resources")]
    [SerializeField] private Transform _airBubbleTransform;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private float _bubbleResponse = 1f;

    private List<Bubble> _bubbles = new();

    private BoxCollider2D _boxCollider;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private LayerMask _groundLayers;
    private LayerMask _bubbleLayers;
    private int _inputHash;

    private BubbleStruct _bubbleStruct;
    private Attributes _attributes;

    private CustomCursor _customCursor;
    private Camera _mainCamera;

    private int _requestedMovement = 0;
    private float _timeSinceGrounded = 0;
    private float _timeSinceJumpRequest = 0;
    private bool _requestedJumpCut = false;
    private bool _requestedShootLeft = false;
    private bool _requestedShootRight = false;
    private bool _requestedSustainedShootLeft = false;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _groundLayers = LayerMask.GetMask("Ground");
        _bubbleLayers = LayerMask.GetMask("Bubble");
        _inputHash = Animator.StringToHash("Input");

        _attributes = new Attributes(_airBubbleTransform, _maxHealth, _bubbleResponse);
        _customCursor = FindFirstObjectByType<CustomCursor>();
        _mainCamera = Camera.main;

        Utils.SetRigidbody(_rigidbody);
    }

    private void OnDisable()
    {
        _attributes.Cleanup();

        foreach (var bubble in _bubbles)
            bubble.OnPop(release: true, explode: false);
    }

    private void Update()
    {
        TickTimers(Time.deltaTime);
        UpdateInput();

        _animator.SetFloat(_inputHash, Mathf.Abs(_requestedMovement));
    }

    private void LateUpdate()
    {
        _attributes.LateTick(Time.deltaTime);

        if (_attributes.CurrentHealth == 0)
            Kill();
    }

    private void FixedUpdate()
    {
        var grounded = PerformGroundCheck();

        if (grounded)
        {
            _timeSinceGrounded = _coyoteTime;
            _rigidbody.gravityScale = _defaultGravity;
        }

        var mouseWorldPos = (Vector2)_mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.OverlapCircle(mouseWorldPos, _bubbleCheckSize, _bubbleLayers);
        _customCursor.UpdateColorAndPosition(hit, _bubbleStruct.Bubble);

        HandleTurns(mouseWorldPos);
        HandleShooting(hit, mouseWorldPos, Time.fixedDeltaTime);

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

        _requestedShootRight = _requestedShootRight || Input.GetMouseButtonDown(1);
        _requestedShootLeft = _requestedShootLeft || Input.GetMouseButtonDown(0);
        _requestedSustainedShootLeft = Input.GetMouseButton(0);
    }

    private void TickTimers(float deltaTime)
    {
        _timeSinceGrounded -= deltaTime;
        _timeSinceJumpRequest -= deltaTime;
    }

    private void HandleTurns(Vector2 mouseWorldPos)
    {
        var cursorX = mouseWorldPos.x;
        var playerX = _rigidbody.position.x;
        var localScale = transform.localScale;

        if (localScale.x > 0 && cursorX < playerX || localScale.x < 0 && cursorX > playerX)
        {
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    private void HandleShooting(Collider2D hit, Vector2 mouseWorldPos, float deltaTime)
    {
        // Calculate dir towards mouth
        var playerPos = _rigidbody.position;
        playerPos.y += _boxCollider.bounds.extents.y;
        var direction = (mouseWorldPos - playerPos).normalized;
        var spawnPosition = playerPos + direction * _spawnOffset;

        if (_bubbleStruct.Bubble)
        {
            // Add charge
            _bubbleStruct.Charge += deltaTime;
            _bubbleStruct.Charge = Mathf.Clamp(_bubbleStruct.Charge, 0, 2f);
            var charge = Mathf.Clamp(_bubbleStruct.Charge, 1, 2);

            _attributes.OnHealthUpdated(_bubbleStruct.Health);
            _attributes.RemoveHealth(_bubbleStruct.Bubble.Volume);

            // Maintain BUBBLED
            if (_requestedSustainedShootLeft)
            {
                // Restrict downward aiming by clamping within allowed regions
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                _bubbleStruct.Bubble.Charge(charge);
                _bubbleStruct.Bubble.transform.SetPositionAndRotation(spawnPosition, Quaternion.Euler(0, 0, angle));
            }
            else
            {
                // Release BUBBLED. Either launch (if no overlap) or dissapate
                var radius = _bubbleStruct.Collider.radius;
                if (!Physics2D.OverlapCircle(spawnPosition, radius, _groundLayers))
                    _bubbleStruct.Bubble.Launch(charge);
                else
                {
                    _attributes.AddHealth(_bubbleStruct.Bubble.Volume);
                    RemoveBubble(_bubbleStruct.Bubble);
                }
                _bubbleStruct.Bubble = null;
            }
        }
        else if (_requestedShootLeft)
        {
            // Spawn popBubble
            var spawnBubble = Instantiate(_bubblePrefab, spawnPosition, Quaternion.identity);
            _bubbleStruct = new()
            {
                Bubble = spawnBubble,
                Collider = spawnBubble.GetComponent<CircleCollider2D>(),
                Health = _attributes.CurrentHealth,
                Charge = 0.5f
            };
            AddBubble(spawnBubble);
        }
        else if (_requestedShootRight)
        {
            // OnPop the bubble
            if (hit && hit.transform.TryGetComponent(out Bubble popBubble))
                popBubble.OnPop(release: true, explode: true);
        }
        _requestedShootRight = false;
        _requestedShootLeft = false;
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

    private void AddBubble(Bubble bubble)
    {
        bubble.Initialize();
        _bubbles.Add(bubble);
        bubble.Pop += Controller_Pop;
    }

    private void RemoveBubble(Bubble bubble)
    {
        _bubbles.Remove(bubble);
        bubble.Pop -= Controller_Pop;
        Destroy(bubble.gameObject);
    }

    private void Controller_Pop(Bubble bubble) => RemoveBubble(bubble);

    public void Kill()
    {
        for (int i = _bubbles.Count - 1; i >= 0; i--)
            _bubbles[i].OnPop(release: true, explode: false);

        var levelManager = FindFirstObjectByType<LevelManager>();
        levelManager.ChangeLevels(levelManager.currentLevel);
    }
}