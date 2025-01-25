using UnityEngine;
using UnityEngine.UIElements;

public class FlyingEnemy : BaseEnemy
{
    /// <summary>
    /// Enemy that flies in a sine wave pattern.
    /// 
    /// Needs to have an enemy spawner set in editor if hasSpawner is true
    /// </summary>

    [Space]
    [Header("Sine Movement")]
    [SerializeField] private float curveSpeed = 5f; // Speed of the sine wave (frequency)
    [SerializeField] private float curveRadius = 2f; // Height of the sine wave (amplitude)
    [SerializeField] private float lateralSpeed = 0.8f; // Lateral speed of the enemy
    private float sinCenterY;

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        sinCenterY = transform.position.y;
        staysOnPlatform = false;
    }

    protected new void FixedUpdate()
    {
        switch (ThisStatus)
        {
            case Status.roaming:
                    if (CheckWall())
                    {
                        Direction *= -1;
                    }
                    SineMovement();
                break;
            case Status.bubble:
                break;
            case Status.stunned:
                StunTimer += Time.deltaTime;
                if (StunTimer >= StunMaxTime)
                {
                    ThisStatus = Status.roaming;
                    StunTimer = 0f;
                }
                break;
            default:
                break;
        }
    }

    void SineMovement()
    {
        // Grab position
        var pos = transform.position;

        // Move in sine wave pattern
        pos.x += Direction * lateralSpeed * Time.deltaTime;
        float sin = Mathf.Sin(pos.x * curveSpeed) * curveRadius;
        pos.y = sinCenterY + sin;

        transform.position = pos;
    }
}
