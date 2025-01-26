namespace Bubbles
{
    using UnityEngine;
    using System;

    public struct SinMovementStruct
    {
        public float Force;
        public float Intensity;
        public float Frequency;
    }

    public struct BubbleStruct
    {
        public Bubble Bubble;
        public CircleCollider2D Collider;
        public float Health;
        public float Charge;
    }

    public class Bubble : MonoBehaviour, IKillable
    {
        public float Volume { get; private set; }

        [Header("SETTINGS")]

        [Space]
        [Header("Charge")]
        [SerializeField] private float _explosionForce = 20f;
        [SerializeField] private float _explosionRadius = 4f;
        [SerializeField] private float _lifeTime = 2f;

        [Space]
        [Header("Movement")]
        [SerializeField] private float _movementForce = 10f;
        [SerializeField] private float _movementIntensity = 0.4f;
        [SerializeField] private float _movementFrequency = 4f;

        [Space]
        [Header("Floating")]
        [SerializeField] private float _floatVelocity = 4f;
        [SerializeField] private float _floatIntensity = 0.4f;
        [SerializeField] private float _floatFrequency = 2;
        [SerializeField] private float _floatSlowdown = 2f;


        [Space]
        [Header("Visuals")]
        [Range(0, 1)]
        [SerializeField] private float _transparency = 0.5f;

        [Space]
        [Header("Audio")]
        [SerializeField] private AudioClip _popClip;

        private Rigidbody2D _rigidbody;
        private Collider2D _collider;
        private Transform _transform;

        private Capture _capture;
        private Explode _explode;
        private Motion _motion;

        private float _adjustedLifeTime;
        private float _lifeTimeLeft;

        public event Action<Bubble> Pop;

        // make this an initialize function instead
        public void Initialize()
        {
            _collider = GetComponent<Collider2D>();
            _collider.enabled = false;

            _transform = transform;
        }

        public void Charge(float charge)
        {
            Volume = Mathf.CeilToInt(charge * 10);
            _transform.localScale = new Vector3(charge, charge, 1);
        }

        public void Launch(float charge)
        {
            Volume = Mathf.CeilToInt(charge * 10);

            _rigidbody = GetComponent<Rigidbody2D>();
            Utils.SetRigidbody(_rigidbody);
            _rigidbody.gravityScale = 0f;
            _rigidbody.mass = 1 * charge;

            _adjustedLifeTime = _lifeTime * charge;
            _lifeTimeLeft = _adjustedLifeTime;

            SinMovementStruct movement = new()
            {
                Force = _movementForce,
                Intensity = _movementIntensity,
                Frequency = _movementFrequency,
            };
            SinMovementStruct floating = new()
            {
                Force = _floatVelocity / charge,
                Intensity = _floatIntensity,
                Frequency = _floatFrequency,
            };

            _capture = new Capture(_transparency);
            _explode = new Explode(_explosionForce * charge, _explosionRadius * charge);
            _motion = new Motion(_rigidbody, movement, floating, _floatSlowdown);

            _capture.Captured += _motion.Motion_ObjectCaptured;

            _collider.enabled = true;
        }

        private void OnDisable()
        {
            if (_capture != null)
                _capture.Captured -= _motion.Motion_ObjectCaptured;
        }

        private void Update()
        {
            if (!_collider.enabled)
                return;

            if (_capture.CaptureTarget)
                _capture.Follow();
            else
            {
                _lifeTimeLeft -= Time.deltaTime;
                if (_lifeTimeLeft <= 0)
                    OnPop(release: true, explode: false);
            }
        }

        private void FixedUpdate()
        {
            if (!_collider.enabled)
                return;

            if (!_capture.CaptureTarget)
                _motion.HandleMovement(_transform.right, _transform.up, _adjustedLifeTime, _lifeTimeLeft);
            else if (_motion.EnableFloat)
                _motion.HandleFloat(Time.fixedDeltaTime);
            else
                _rigidbody.linearDamping = 2;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!_capture.CaptureTarget && collision.transform.TryGetComponent(out BaseEnemy enemy) && enemy.CurrentState is State.ROAMING)
                _capture.OnCaptured(transform, collision.transform);
            else if (collision.transform.TryGetComponent(out Controller controller))
            {
                controller.Attributes.AddHealth(Volume);
                var explode = controller.Rigidbody.linearVelocity.y < 0;
                OnPop(release: true, explode: explode);
            }
        }

        public void OnPop(bool release, bool explode)
        {
            AudioManager.Instance.PlayClip(_popClip);
            Pop?.Invoke(this);

            if (explode)
                _explode.CastExplosion(_rigidbody, (Vector2)_transform.position);

            var target = _capture.CaptureTarget;
            if (target)
            {
                _capture.OnReleased();

                if (!release)
                    target.GetComponent<IKillable>().Kill();
            }

            Destroy(gameObject);
        }

        public void HandleExplosion() => _motion.EnableFloat = false;
        public void Kill() => OnPop(release: false, explode: true);
    }
}