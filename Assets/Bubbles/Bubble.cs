namespace Bubble
{
    using UnityEngine;

    public struct SinMovementStruct
    {
        public float Force;
        public float Intensity;
        public float Frequency;
    }

    public class Bubble : MonoBehaviour, IKillable
    {
        // Remove this later in favor of passing it while initializing. Clamp to 1 and 2 (or other max value)
        [Range(1f, 2f)]
        [SerializeField] private float _charge;

        [Header("SETTINGS")]

        [Space]
        [Header("Charge")]
        [SerializeField] private float _explosionForce = 20f;
        [SerializeField] private float _explosionRadius = 4f;
        [SerializeField] private float _lifeTime = 2f;

        [Space]
        [Header("Movement")]
        [SerializeField] private float _movementForce = 10f;
        [SerializeField] private float _movementIntensity = 0.2f;
        [SerializeField] private float _movementFrequency = 4f;

        [Space]
        [Header("Floating")]
        [SerializeField] private float _floatVelocity = 4f;
        [SerializeField] private float _floatIntensity = 0.5f;
        [SerializeField] private float _floatFrequency = 2;
        [SerializeField] private float _floatSlowdown = 2f;


        [Space]
        [Header("Visuals")]
        [Range(0, 1)]
        [SerializeField] private float _transparency = 0.5f;

        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private Capture _capture;
        private Explode _explode;
        private Motion _motion;

        private float _adjustedLifeTime;
        private float _lifeTimeLeft;

        // make this an initialize function instead
        private void Start()
        {
            _adjustedLifeTime = _lifeTime * _charge;

            _rigidbody = GetComponent<Rigidbody2D>();
            _transform = transform;

            Utils.SetRigidbody(_rigidbody);
            _rigidbody.gravityScale = 0f;
            _rigidbody.mass = 1 * _charge;

            _lifeTimeLeft = _adjustedLifeTime;

            SinMovementStruct movement = new()
            {
                Force = _movementForce,
                Intensity = _movementIntensity,
                Frequency = _movementFrequency,
            };
            SinMovementStruct floating = new()
            {
                Force = _floatVelocity / _charge,
                Intensity = _floatIntensity,
                Frequency = _floatFrequency,
            };

            _capture = new Capture(_transparency);
            _explode = new Explode(_explosionForce * _charge, _explosionRadius * _charge);
            _motion = new Motion(_rigidbody, movement, floating, _floatSlowdown);

            _capture.Captured += _motion.Motion_ObjectCaptured;
        }

        private void OnDisable()
        {
            _capture.Cleanup();
        }

        private void Update()
        {
            if (_capture.HasCapture)
                _capture.Follow();
            else
            {
                _lifeTimeLeft -= Time.deltaTime;
                if (_lifeTimeLeft <= 0)
                    Pop(release: true, explode: false);
            }
        }

        private void FixedUpdate()
        {
            if (!_capture.HasCapture)
                _motion.HandleMovement(_transform.right, _transform.up, _adjustedLifeTime, _lifeTimeLeft);
            else
                _motion.HandleFloat(Time.fixedDeltaTime);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!_capture.HasCapture && collision.gameObject.CompareTag("Capturable"))
                _capture.OnCaptured(_transform, collision.transform);
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

        private void Pop(bool release, bool explode)
        {
            if (explode)
                _explode.CastExplosion(_rigidbody, (Vector2)_transform.position);

            if (release && _capture.HasCapture)
                _capture.OnReleased();

            Destroy(gameObject);
        }

        public void Kill() => Pop(release: false, explode: true);
    }
}