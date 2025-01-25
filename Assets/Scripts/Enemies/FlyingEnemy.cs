using UnityEngine;
using UnityEngine.UIElements;

public class FlyingEnemy : MonoBehaviour, IKillable
{
    /// <summary>
    /// Enemy that flies in a sine wave pattern.
    /// 
    /// Needs to have an enemy spawner set in editor if hasSpawner is true
    /// </summary>

    private Rigidbody2D _rigidbody;
    public EnemySpawner _spawner;

    public bool hasSpawner = false;
    [SerializeField] private int enemyId = 0; //Used to match this enemy to a corresponding spawner

    [Space]
    [Header("Sine Movement")]
    [SerializeField] private float curveSpeed = 5f; // Speed of the sine wave (frequency)
    [SerializeField] private float curveRadius = 2f; // Height of the sine wave (amplitude)
    [SerializeField] private float currentSpeed = 0.1f; // Lateral speed of the enemy
    private float sinCenterY;
    private float direction = -1; //1 for left, -1 for right

    [Space]
    [SerializeField] private bool die = false;
    public enum Status
    {
        roaming,
        bubble,
        stunned
    }
    [SerializeField] private Status status = Status.roaming;

    void Start()
    {
        sinCenterY = transform.position.y;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (CheckWall())
        {
            direction *= -1;
        }
        SineMovement();
    }

    void SineMovement()
    {
        // Grab position
        var pos = transform.position;

        // Move in sine wave pattern
        pos.x += direction * currentSpeed * Time.deltaTime;
        float sin = Mathf.Sin(pos.x * curveSpeed) * curveRadius;
        pos.y = sinCenterY + sin;

        transform.position = pos;
    }

    bool CheckWall()
    {
        var origin = _rigidbody.position;
        var dir = new Vector2(direction,0);
        //Debug.DrawRay(origin, dir, Color.blue, 50f);
        return Physics2D.Raycast(origin, dir, dir.magnitude, LayerMask.GetMask("Ground")); //dir.magnitude
    }

    void Death()
    {
        // Death logic
    }

    void IKillable.OnSpikeHit()
    {
        Death();
    }
}
