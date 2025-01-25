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

    protected override void Start()
    {
        base.Start();

        sinCenterY = transform.position.y;
    }

    protected new void FixedUpdate()
    {
        switch (_CurrentState)
        {
            case State.ROAMING:
                    if (CheckWall())
                    {
                        _Direction *= -1;
                    }
                    SineMovement();
                break;
            case State.BUBBLED:
                break;
            case State.STUNNED:
                _StunTimer += Time.deltaTime;
                if (_StunTimer >= _TotalStunTime)
                {
                    _CurrentState = State.ROAMING;
                    _StunTimer = 0f;
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
        pos.x += _Direction * lateralSpeed * Time.deltaTime;
        float sin = Mathf.Sin(pos.x * curveSpeed) * curveRadius;
        pos.y = sinCenterY + sin;

        transform.position = pos;
    }
}
