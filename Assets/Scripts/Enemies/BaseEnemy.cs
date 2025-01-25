using UnityEngine;

public enum Status
{
    ROAMING,
    BUBBLE,
    STUNNED
}

public class BaseEnemy : MonoBehaviour, IKillable
{
    private Rigidbody2D _rigidbody;
    [HideInInspector] public Rigidbody2D Rigidbody
    {
        get => _rigidbody;
        set => _rigidbody = value;
    }
    public EnemySpawner _spawner;

    public bool hasSpawner = false;

    public bool staysOnPlatform; //If false, skips the check to turn around at edges
    [SerializeField] private int enemyId = 0; //Used to match this enemy to a corresponding spawner
    [HideInInspector] public int EnemyId
    {
        get => enemyId;
        set => enemyId = value;
    }

    [Space]
    [Header("Movement")]
    [SerializeField] private float acceleration = 0.1f;
    [HideInInspector] public float Acceleration
    {
        get => acceleration;
        set => acceleration = value;
    }
    private float currentSpeed = 0f;
    [HideInInspector] public float CurrentSpeed
    {
        get => currentSpeed;
        set => currentSpeed = value;
    }
    [SerializeField] private float maxSpeed = 3f;
    [HideInInspector] public float MaxSpeed
    {
        get => maxSpeed;
        set => maxSpeed = value;
    }
    private float direction = -1; //1 for left, -1 for right
    [HideInInspector] public float Direction
    {
        get => direction;
        set => direction = value;
    }

    [HideInInspector] public bool die = false;

    [SerializeField] private Status _currentState = Status.ROAMING;
    [HideInInspector] public Status ThisStatus
    {
        get => _currentState;
        set => _currentState = value;
    }
    [SerializeField] private float stunMaxTime = 1f;
    [HideInInspector] public float StunMaxTime
    {
        get => stunMaxTime;
        set => stunMaxTime = value;
    }
    [SerializeField] private float stunTimer = 0f;
    [HideInInspector] public float StunTimer
    {
        get => stunTimer;
        set => stunTimer = value;
    }

    private SpriteRenderer _spriteRenderer;
    private LayerMask _groundLayers;
    private Vector2 _startPosition;

    private void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _groundLayers = LayerMask.GetMask("Ground");
        _startPosition = transform.position;

        if (direction == -1)
            _spriteRenderer.flipY = !_spriteRenderer.flipY;

        LevelManager.resetGameState += LevelManager_resetGameState;
    }

    protected virtual void FixedUpdate()
    {
        switch (_currentState)
        {
            case Status.ROAMING:
                if ((staysOnPlatform && CheckEdge()) || CheckWall()) 
                {
                    direction *= -1;
                    currentSpeed = 0f;
                    _spriteRenderer.flipY = !_spriteRenderer.flipY;
                }
                currentSpeed = Mathf.Clamp(currentSpeed + acceleration, 0, MaxSpeed);
                _rigidbody.linearVelocity = new Vector2(currentSpeed * direction, _rigidbody.linearVelocity.y);
                break;
            case Status.BUBBLE:
                break;
            case Status.STUNNED:
                HandleStuns();
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.TryGetComponent(out Controller controller))
            controller.Kill();
    }

    private void LevelManager_resetGameState() => transform.position = _startPosition;

    protected void HandleStuns()
    {
        stunTimer += Time.deltaTime;
        if (stunTimer >= stunMaxTime)
        {
            _currentState = Status.ROAMING;
            stunTimer = 0f;
        }
    }

    protected bool CheckEdge()
    {
        var pos = _rigidbody.position;
        pos.x += direction;
        var dir = Vector2.down;
        return !Physics2D.Raycast(pos, dir, dir.magnitude, _groundLayers);
    }

    protected bool CheckWall()
    {
        var origin = _rigidbody.position;
        var dir = new Vector2(direction, 0);
        return Physics2D.Raycast(origin, dir, dir.magnitude, _groundLayers);
    }

    public void Kill() => Destroy(gameObject);
}