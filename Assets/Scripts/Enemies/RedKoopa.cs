using System;
using UnityEngine;

public class RedKoopa : MonoBehaviour, IKillable
{
    /// <summary>
    /// Enemy that walks left and right, but will not voluntarily drop off the platform it is on
    /// Could also be made so it can ignore ledges and just walk left and right but don't know if we'll use that
    /// 
    /// Needs to have an enemy spawner set in editor if hasSpawner is true
    /// </summary>

    private Rigidbody2D _rigidbody;
    public EnemySpawner _spawner;

    public bool hasSpawner = false;
    [SerializeField] private int enemyId = 0; //Used to match this enemy to a corresponding spawner
    private bool staysOnPlatform = true; //If false, skips the check to turn around at edges

    private float acceleration = 0.1f;
    private float currentSpeed = 0f;
    private float maxSpeed = 3f;
    private float direction = -1; //1 for left, -1 for right

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
                break;
            case Status.bubble:
                break;
            case Status.stunned:
                break;
            default:
                break;
        }
        if (die)
        {
            Death();
        }
    }

    // It can do a lil death animation maybe, like falling down off-the-screen while spinning to keep it simple. In any case, it dies
    public void Death()
    {
        Destroy(gameObject);

        /*
        //Was trying to do a death animation, wasn't working so shelving this for now
        status = "stunned";
        GetComponent<Collider2D>().enabled = false;
        _rigidbody.linearVelocity = new Vector2(0, _rigidbody.linearVelocity.y);
        _rigidbody.AddForce(new Vector2(0, 50f));
        */
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

    void IKillable.OnSpikeHit()
    {
        Death();
    }
}
