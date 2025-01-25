namespace Bubble
{
    using UnityEngine;

    public class Motion
    {
        private readonly Rigidbody2D _rigidbody;
        private readonly float _initialForce;
        private readonly float _frequency;
        private readonly float _intensity;
        private float _force;

        public Motion(Rigidbody2D rigidbody, float initialForce, float frequency, float intensity)
        {
            _rigidbody = rigidbody;
            _initialForce = initialForce;
            _frequency = frequency;
            _intensity = intensity;

            _force = initialForce;
        }

        public void HandleMovementForce(Vector3 rightDirection, Vector3 upDirection, float lifeTime, float currentTime)
        {
            //Calculate sine wave oscillation along the "up" direction
            var wave = Mathf.Sin(currentTime * _frequency) * _rigidbody.linearVelocity.magnitude * _intensity;
            _rigidbody.linearVelocity = (Vector2)upDirection * wave;

            _force = Mathf.Lerp(0, _initialForce, currentTime / lifeTime);
            _rigidbody.AddForce(rightDirection * _force, ForceMode2D.Impulse);

            if (_rigidbody.linearVelocity.magnitude < 0.05f)
                _rigidbody.linearVelocity = Vector2.zero;
        }
    }
}