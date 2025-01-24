using UnityEngine;

public class RedKoopa : MonoBehaviour, IKillable
{
    /// <summary>
    /// Enemy that walks left and right, but will not voluntarily drop off the platform it is on
    /// Could also be made so it can ignore ledges and just walk left and right but don't know if we'll use that
    /// </summary>

    private Rigidbody2D _rigibody;

    private float speed;
    private float direction;

    
    [SerializeField] private string status = "roaming"; //roaming means walking around, bubble means in a bubble (so doing nothing), stunned means right after being released from a bubble it cannot move for a bit

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    // It can do a lil death animation maybe, like falling down off-the-screen while spinning to keep it simple. In any case, it dies
    public void death()
    {

    }

    void CheckEdge()
    {
        var origin = _rigibody.position + _rigibody.linearVelocity.normalized;
        var dir = Vector2.down;

        if (Physics2D.Raycast(origin, dir, 2f, LayerMask.GetMask(" Ground")))
        {

        }
        else
        {

        }
    }

    void IKillable.OnSpikeHit()
    {
        death();
    }
}
