namespace Bubble
{
    using UnityEngine;

    public class Bubble : MonoBehaviour, IKillable
    {
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

        private float _adjustedLifeTime;

        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private float _lifeTimeLeft;

        private Capture _capture;
        private Explode _explode;
        private Motion _motion;

        // make this an initialize function instead
        private void Start()
        {
            _adjustedLifeTime = _lifeTime * _charge;

            _rigidbody = GetComponent<Rigidbody2D>();
            _transform = transform;

            Utils.SetRigidbody(_rigidbody);
            _rigidbody.gravityScale = 0;
            _rigidbody.mass = _mass * _charge;

            _lifeTimeLeft = _adjustedLifeTime;

            _capture = new Capture(_transparency);
            _explode = new Explode(_explosionForce * _charge, _explosionRadius * _charge);
            _motion = new Motion(_rigidbody, _initialForce, _frequency, _intensity);
        }

        private void Update()
        {
            _lifeTimeLeft -= Time.deltaTime;

            if (_capture.HasCapture)
                _capture.Follow();
            else if (_lifeTimeLeft <= 0)
                Pop(release: true, explode: false);
        }

        private void FixedUpdate()
        {
            _motion.HandleMovementForce(_transform.right, _transform.up, _adjustedLifeTime, _lifeTimeLeft);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Capturable"))
                _capture.CaptureObject(_transform, collision.transform);
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
                _capture.ReleaseObject();

            Destroy(gameObject);
        }

        public void OnSpikeHit() => Pop(release: false, explode: true);
    }
}