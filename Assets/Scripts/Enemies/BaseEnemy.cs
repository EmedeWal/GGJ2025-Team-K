using UnityEngine;

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

    public enum Status
    {
        roaming,
        bubble,
        stunned
    }
    [SerializeField] private Status thisStatus = Status.roaming;
    [HideInInspector] public Status ThisStatus
    {
        get => thisStatus;
        set => thisStatus = value;
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


    void Start()
    {
    }

    protected virtual void FixedUpdate()
    {
        switch (thisStatus)
        {
            case Status.roaming:
                if (staysOnPlatform)
                {
                    if (CheckEdge() || CheckWall()) //About to fall off platform  
                    {
                        Debug.Log("hit wall/edge");
                        direction *= -1;
                        currentSpeed = 0f;
                    }
                }
                currentSpeed = Mathf.Clamp(currentSpeed + acceleration, 0, MaxSpeed);
                _rigidbody.linearVelocity = new Vector2(currentSpeed * direction, _rigidbody.linearVelocity.y);
                break;
            case Status.bubble:
                break;
            case Status.stunned:
                HandleStuns();
                break;
            default:
                break;
        }
        if (die)
        {
            Death();
        }

    }

    public bool CheckEdge()
    {
        var pos = _rigidbody.position;
        pos.x += direction;
        var dir = Vector2.down;
        //Debug.DrawRay(pos, dir, Color.red, 50f);
        return !Physics2D.Raycast(pos, dir, dir.magnitude, LayerMask.GetMask("Ground"));
    }

    public bool CheckWall()
    {
        var origin = _rigidbody.position;
        var dir = new Vector2(direction,0);
        //Debug.DrawRay(origin, dir, Color.blue, 50f);
        return Physics2D.Raycast(origin, dir, dir.magnitude, LayerMask.GetMask("Ground")); //dir.magnitude
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    public void HandleStuns()
    {
        if (_rigidbody.linearVelocity.y < 0)
        {
            return;
        }
        stunTimer += Time.deltaTime;
        if (stunTimer >= stunMaxTime)
        {
            thisStatus = Status.roaming;
            stunTimer = 0f;
        }
    }

    void IKillable.OnSpikeHit()
    {
        Death();
    }

}
