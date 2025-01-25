using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public struct CaptureStruct
{ 
    public Transform Transform;
    public Collider2D Collider;
    public Rigidbody2D Rigidbody;
    public SpriteRenderer Renderer;
}

public class Bubble : MonoBehaviour, IKillable
{
    private CaptureStruct _struct;

    // Remove this later in favor of passing it while initializing. Clamp to 1 and 2 (or other max value)
    [Range(1f, 2f)]
    [SerializeField] private float _charge;

    [Header("SETTINGS")]

    [Space]
    [Header("Charge")]
    [SerializeField] private float _mass = 1f;
    [SerializeField] private float _explosionForce = 20f;
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private float _lifeTime = 3f;

    [Space]
    [Header("Movement")]
    [SerializeField] private float _initialForce = 30f;
    [SerializeField] private float _intensity = 3f;
    [SerializeField] private float _frequency = 3f;

    [Space]
    [Header("Visuals")]
    [Range(0, 1)]
    [SerializeField] private float _transparency;

    private float _adjustedMass;
    private float _adjustedLifeTime;
    private float _adjustedExplosionForce;
    private float _adjustedExplosionRadius;

    private Rigidbody2D _rigidbody;
    private Transform _transform;
    private float _timer;
    private float _force;

    // make this an initialize function instead
    private void Start()
    {
        _adjustedExplosionRadius = _adjustedExplosionRadius * _charge;
        _adjustedExplosionForce = _explosionForce * _charge;
        _adjustedLifeTime = _lifeTime * _charge;
        _adjustedMass = _mass * _charge;

        _rigidbody = GetComponent<Rigidbody2D>();
        _transform = transform;
        
        Utils.SetRigidbody(_rigidbody);
        _rigidbody.gravityScale = 0;
        _rigidbody.mass = _adjustedMass;

        _timer = _adjustedLifeTime;
        _force = _initialForce;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_struct.Transform)
            _struct.Transform.localPosition = Vector2.zero;
        else if (_timer <= 0)
            Pop(release: true, explode: false);
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Capturable"))
            CaptureObject(collision.transform);
        else if (collision.transform.TryGetComponent(out Controller controller))
            Pop(release: true, explode: controller.Rigidbody.linearVelocity.y < 0);
        else
            Pop(release: true, explode: true);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
            Pop(release: true, explode: true);
    }

    private void HandleMovement()
    {
        //Calculate sine wave oscillation along the "up" direction
        var wave = Mathf.Sin(_timer * _frequency) * _rigidbody.linearVelocity.magnitude * _intensity;
        var up = new Vector2(_transform.up.x, _transform.up.y);
        _rigidbody.linearVelocity = up * wave;

        _force = Mathf.Lerp(0, _initialForce, _timer / _adjustedLifeTime);
        _rigidbody.AddForce(_transform.right * _force, ForceMode2D.Impulse);
        Debug.Log(_timer / _adjustedLifeTime);

        if (_rigidbody.linearVelocity.magnitude < 0.05f)
            _rigidbody.linearVelocity = Vector2.zero;
    }

    private void CaptureObject(Transform capturedTransform)
    {
        _struct = new()
        {
            Transform = capturedTransform,
            Collider = capturedTransform.GetComponent<Collider2D>(),
            Rigidbody = capturedTransform.GetComponent<Rigidbody2D>(),
            Renderer = capturedTransform.GetComponentInChildren<SpriteRenderer>(),
        }; 

        ChangeCapturedObjectAlpha(_transparency);

        _struct.Transform.SetParent(transform);
        _struct.Transform.localPosition = Vector2.zero;
        _struct.Collider.enabled = false;
        _struct.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void ReleaseObject()
    {
        ChangeCapturedObjectAlpha(1);

        _struct.Transform.SetParent(null);
        _struct.Collider.enabled = true;
        _struct.Rigidbody.constraints = RigidbodyConstraints2D.None;
        _struct.Transform = null;
    }

    private void ChangeCapturedObjectAlpha(float alpha)
    {
        var colour = _struct.Renderer.color;
        colour.a = alpha;
        _struct.Renderer.color = colour;
    }

    private void CastExplosion()
    {
        var colliders = Physics2D.OverlapCircleAll(_transform.position, _explosionRadius);
        foreach (var coll in colliders)
            if (coll.TryGetComponent(out Rigidbody2D rigidbody) && rigidbody != _rigidbody)
                AddExplosionForce(rigidbody, _adjustedExplosionForce, _transform.position, _adjustedExplosionRadius);
    }

    private void AddExplosionForce(Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius, ForceMode2D mode = ForceMode2D.Impulse)
    {
        var explosionDir = rb.position - explosionPosition;
        var explosionDistance = explosionDir.magnitude;
        explosionDir.Normalize();

        if (explosionDir.sqrMagnitude > 0)
        {
            var forceMagnitude = Mathf.Lerp(0, explosionForce, 1 - (explosionDistance / explosionRadius));
            var force = forceMagnitude * explosionDir;
            rb.AddForce(force, mode);
        }
    }

    private void Pop(bool release, bool explode)
    {
        if (explode)
            CastExplosion();

        if (release && _struct.Transform)
            ReleaseObject();

        Destroy(gameObject);
    }

    public void OnSpikeHit() => Pop(release: false, explode: true);
}