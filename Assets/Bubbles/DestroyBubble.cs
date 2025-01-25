using UnityEditor;
using UnityEngine;

public class DestroyBubble : MonoBehaviour
{
    [Header("SETTINGS")]
    [Space]
    [Header("Explosion")]
    [SerializeField] private float _explosionForce;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _upwardsModifier;


    private Rigidbody2D _rigidbody;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);
            foreach (Collider2D h in colliders)
            {
                Rigidbody2D temprb = h.GetComponent<Rigidbody2D>();
                if (temprb != null && temprb != _rigidbody) 
                {
                    print(temprb.name);
                    AddExplosionForce(temprb, _explosionForce, transform.position, _explosionRadius, _upwardsModifier);
                }
            }
            if (GetComponent<CaptureObject>().IsCaptured) GetComponent<CaptureObject>().OnRelease();
            Destroy(gameObject);
        }
    }

    void AddExplosionForce(Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier = 0.0f, ForceMode2D mode = ForceMode2D.Force)
    {
        var explosionDir = rb.position - explosionPosition;
        var explosionDistance = explosionDir.magnitude;

        // Normalize without computing magnitude again
        if (upwardsModifier == 0)
            explosionDir /= explosionDistance;
        else {
            // From Rigidbody.AddExplosionForce doc:
            // If you pass a non-zero value for the upwardsModifier parameter, the direction
            // will be modified by subtracting that value from the Y component of the centre point.
            explosionDir.y += upwardsModifier;
            explosionDir.Normalize();
        }

    print("Explosion Direction: " + explosionDir);
    print("Explosion Distance: " + explosionDistance);
    print("Explosion Force: " + explosionForce);
    print("Explosion Radius: " + explosionRadius);
    print("Force Mode: " + mode);
    print("Rigidbody Body Type: " + rb.bodyType);

    float forceMagnitude = Mathf.Lerp(0, explosionForce, 1 - (explosionDistance / explosionRadius));
    Vector2 force = forceMagnitude * explosionDir;

    print("Calculated Force Magnitude: " + forceMagnitude);
    print("Calculated Force: " + force);

    print(rb.constraints);

    if (rb.bodyType != RigidbodyType2D.Kinematic)
    {
        rb.AddForce(force, mode);
        print("Force applied to Rigidbody.");
    }
    else
    {
        print("Rigidbody is kinematic, no force applied.");
    }
    }
}
