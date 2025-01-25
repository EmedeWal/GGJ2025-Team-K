using UnityEngine;

public enum State
{
    ROAMING,
    STUNNED,
    BUBBLED,
}

public class BaseEnemy : MonoBehaviour, IKillable
{
    [Header("SETTINGS")]    

    [Space]
    [Header("Movement")]
    [SerializeField] protected float _Acceleration = 0.1f;
    [SerializeField] protected float _MaxSpeed = 3f;
    [SerializeField] protected float _Direction = -1;
    protected float _CurrentSpeed;

    [Space]
    [Header("States")]
    [SerializeField] protected State _CurrentState = State.ROAMING;
    [SerializeField] protected float _TotalStunTime = 1f;
    protected float _StunTimer;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    private LayerMask _groundLayers;
    private Vector2 _startPosition;

    protected virtual void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>(); 
        _groundLayers = LayerMask.GetMask("Ground");
        _startPosition = transform.position;

        if (_Direction < 0)
            _spriteRenderer.flipY = !_spriteRenderer.flipY;

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
                    _spriteRenderer.flipY = !_spriteRenderer.flipY;
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

    private void LevelManager_resetGameState() => transform.position = _startPosition;

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

    public void UpdateState(State state)
    {
        _CurrentState = state;

        if (state is State.STUNNED)
            _StunTimer = _TotalStunTime;
    }

    public void Kill() => Destroy(gameObject);
}