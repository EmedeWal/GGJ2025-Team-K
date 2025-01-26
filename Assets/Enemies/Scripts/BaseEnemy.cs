using UnityEngine;

public enum State
{
    ROAMING,
    STUNNED,
    BUBBLED,
}

public class BaseEnemy : MonoBehaviour, IKillable
{
    public State CurrentState => _CurrentState;

    [Header("SETTINGS")]    

    [Space]
    [Header("Movement")]
    [SerializeField] protected float _Acceleration = 0.1f;
    [SerializeField] protected float _MaxSpeed = 3f;
    [SerializeField] protected float _Direction = -1;
    protected float _CurrentSpeed;

    [Space]
    [Header("States")]
    [SerializeField] protected float _TotalStunTime = 1f;
    protected State _CurrentState = State.ROAMING;
    protected float _StunTimer;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private LayerMask _groundLayers;
    private Vector2 _startPosition;

    protected virtual void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>(); 
        _collider = GetComponent<Collider2D>(); 
        _groundLayers = LayerMask.GetMask("Ground");
        _startPosition = transform.position;

        if (_Direction < 0)
            _spriteRenderer.flipX = !_spriteRenderer.flipX;

        LevelManager.ResetGameState += LevelManager_resetGameState;
    }

    protected virtual void FixedUpdate()
    {
        switch (_CurrentState)
        {
            case State.ROAMING:
                if (CheckEdge() || CheckWall())
                {
                    _Direction *= -1;
                    _CurrentSpeed = 0f;
                    _spriteRenderer.flipX = !_spriteRenderer.flipX;
                }
                _CurrentSpeed = Mathf.Clamp(_CurrentSpeed + _Acceleration, 0, _MaxSpeed);
                _rigidbody.linearVelocity = new Vector2(_CurrentSpeed * _Direction, _rigidbody.linearVelocity.y);
                break;
            case State.BUBBLED:
                break;
            case State.STUNNED:
                HandleStuns();
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_CurrentState is State.ROAMING && collision.transform.TryGetComponent(out Controller controller))
            controller.Kill();
    }

    private void LevelManager_resetGameState() => ToggleActive(true);

    protected void HandleStuns()
    {
        _StunTimer -= Time.deltaTime;
        if (_StunTimer <= 0)
            _CurrentState = State.ROAMING;
    }

    protected bool CheckEdge()
    {
        var pos = _rigidbody.position;
        pos.x += _Direction;
        var dir = Vector2.down;
        return !Physics2D.Raycast(pos, dir, dir.magnitude, _groundLayers);
    }

    protected bool CheckWall()
    {
        var origin = _rigidbody.position;
        var dir = new Vector2(_Direction, 0);
        return Physics2D.Raycast(origin, dir, dir.magnitude, _groundLayers);
    }

    public void Bubble() => _CurrentState = State.BUBBLED;

    public void Stun()
    {
        _CurrentState = State.STUNNED;
        _StunTimer = _TotalStunTime;
    }

    public void Kill() => ToggleActive(false);

    private void ToggleActive(bool active)
    {
        var state = active
            ? State.ROAMING
            : State.BUBBLED;
        _CurrentState = state;

        transform.position = _startPosition;

        _collider.enabled = active;

        var color = _spriteRenderer.color;
        color.a = active ? 1 : 0;
        _spriteRenderer.color = color;
    }
}