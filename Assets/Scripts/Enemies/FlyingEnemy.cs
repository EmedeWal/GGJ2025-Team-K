using UnityEngine;
using UnityEngine.UIElements;

public class FlyingEnemy : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    public EnemySpawner _spawner;

    public bool hasSpawner = false;
    [SerializeField] private int enemyId = 0; //Used to match this enemy to a corresponding spawner

    [SerializeField] private float curveSpeed = 5f; 
    [SerializeField] private float curveRadius = 2f;
    [SerializeField] private float currentSpeed = 0.1f;
    private float sinCenterY;
    private float direction = -1; //1 for left, -1 for right

    [SerializeField] private bool die = false;
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
        var pos = transform.position;

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
}
