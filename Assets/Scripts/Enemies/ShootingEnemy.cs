using UnityEngine;

public class ShootingEnemy : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    public EnemySpawner _spawner;

    public bool hasSpawner = false;
    [SerializeField] private int enemyId = 0; //Used to match this enemy to a corresponding spawner
    private bool staysOnPlatform = true; //If false, skips the check to turn around at edges

    [Space]
    [Header("Movement")]
    [SerializeField] private float acceleration = 0.1f;
    private float currentSpeed = 0f;
    [SerializeField] private float maxSpeed = 3f;
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

    [Space]
    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float fireRate = 1f;
    private float fireTimer = 0f;

    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        switch (status)
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
                currentSpeed = Mathf.Clamp(currentSpeed + acceleration, 0, maxSpeed);
                _rigidbody.linearVelocity = new Vector2(currentSpeed * direction, _rigidbody.linearVelocity.y);

                fireTimer += Time.deltaTime;
                if (fireTimer >= fireRate)
                {
                    Shoot();
                    fireTimer = 0;
                }
                break;
            case Status.bubble:
                break;
            case Status.stunned:
                break;
            default:
                break;
        }
    }

    //Returns true if the enemy is about to drop off an edge
    bool CheckEdge()
    {
        var pos = _rigidbody.position;
        pos.x += direction;
        var dir = Vector2.down;
        //Debug.DrawRay(pos, dir, Color.red, 50f);
        return !Physics2D.Raycast(pos, dir, dir.magnitude, LayerMask.GetMask("Ground"));
    }

    bool CheckWall()
    {
        var origin = _rigidbody.position;
        var dir = new Vector2(direction,0);
        //Debug.DrawRay(origin, dir, Color.blue, 50f);
        return Physics2D.Raycast(origin, dir, dir.magnitude, LayerMask.GetMask("Ground")); //dir.magnitude
    }

    void Shoot()
    {
        var offset = transform.position.x + 0.75f*direction;
        Vector3 pos = new Vector3(offset, transform.position.y, 0);
        GameObject bullet = Instantiate(bulletPrefab, pos, bulletPrefab.transform.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(new Vector2(bulletSpeed * direction, 0));
    }
}
