using UnityEngine;

public class Bubble : MonoBehaviour, IKillable
{
    private Transform _capturedTransform;

    [Header("SETTINGS")]

    [Space]
    [Header("Movement")]
    [SerializeField] private float _initialForce = 30f;
    [SerializeField] private float _response = 3f;
    [SerializeField] private float _intensity = 3f;
    [SerializeField] private float _frequency = 3f;

    [Space]
    [Header("Explosion")]
    [SerializeField] private float _explosionForce = 20f;
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private float _upwardsModifier = 0f;

    [Space]
    [Header("Lifetime")]
    [SerializeField] private float _lifeTime = 3f;

    [Space]
    [Header("Visuals")]
    [Range(0, 1)]
    [SerializeField] private float _transparency;

    private Rigidbody2D _rigidbody;
    private Transform _transform;
    private float _timer = 0;

    private float _force;

    // make this an initialize function instead
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _transform = transform;
        
        Utils.SetRigidbody(_rigidbody);

        _force = _initialForce;
    }

    private void Update()
    {
        // If no object is captured, tick life timer
        if (!_capturedTransform)
        {
            _timer += Time.deltaTime;
            if (_timer > _lifeTime)
                Pop(true);
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, _explosionRadius);
            foreach (Collider2D h in colliders)
            {
                Rigidbody2D temprb = h.GetComponent<Rigidbody2D>();
                if (temprb != null && temprb != _rigidbody)
                    AddExplosionForce(temprb, _explosionForce, _transform.position, _explosionRadius, _upwardsModifier);
            }
            Pop(true);
        }
    }

    private void HandleMovement()
    {
        //Calculate sine wave oscillation along the "up" direction
        var wave = Mathf.Sin(_timer * _frequency) * _rigidbody.linearVelocity.magnitude * _intensity;
        var up = new Vector2(_transform.up.x, _transform.up.y);
        _rigidbody.linearVelocity = up * wave;

        var response = 1f - Mathf.Exp(-_response * Time.fixedDeltaTime);
        _force = Mathf.Lerp(_force, 0, response);
        _rigidbody.AddForce(_transform.right * _force, ForceMode2D.Impulse);

        if (_rigidbody.linearVelocity.magnitude < 0.05f)
            _rigidbody.linearVelocity = Vector2.zero;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Capturable"))
            CaptureObject(collision.transform);
    }

    private void CaptureObject(Transform capturedTransform)
    {
        _capturedTransform = capturedTransform;

        ChangeCapturedObjectAlpha(_transparency);

        _capturedTransform.transform.SetParent(transform);
        _capturedTransform.transform.localPosition = Vector2.zero;
        _capturedTransform.GetComponent<Collider2D>().enabled = false;
    }

    private void ReleaseObject()
    {
        ChangeCapturedObjectAlpha(1);

        _capturedTransform.transform.SetParent(null);
        _capturedTransform.GetComponent<Collider2D>().enabled = true;
    }

    private void ChangeCapturedObjectAlpha(float alpha)
    {
        var renderer = _capturedTransform.GetComponentInChildren<SpriteRenderer>();
        var colour = renderer.color;
        colour.a = alpha;
        renderer.color = colour;
    }

    private void AddExplosionForce(Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier = 0.0f, ForceMode2D mode = ForceMode2D.Impulse)
    {
        var explosionDir = rb.position - explosionPosition;
        var explosionDistance = explosionDir.magnitude;

        // Normalize without computing magnitude again
        if (upwardsModifier == 0)
            explosionDir /= explosionDistance;
        else
        {
            // From Rigidbody.AddExplosionForce doc:
            // If you pass a non-zero value for the upwardsModifier parameter, the direction
            // will be modified by subtracting that value from the Y component of the centre point.
            explosionDir.y += upwardsModifier;
            explosionDir.Normalize();
        }

        float forceMagnitude = Mathf.Lerp(0, explosionForce, 1 - (explosionDistance / explosionRadius));
        Vector2 force = forceMagnitude * explosionDir;
        rb.AddForce(force, mode);
    }

    private void Pop(bool release)
    {
        if (_capturedTransform != null && release)
            ReleaseObject();

        Destroy(gameObject);
    }

    public void OnSpikeHit() => Pop(false);
}