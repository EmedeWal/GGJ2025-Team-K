namespace Bubbles
{
    using UnityEngine;

    public class Motion
    {
        private readonly Rigidbody2D _rigidbody;
        private readonly SinMovementStruct _movement;
        private readonly SinMovementStruct _floating;
        private readonly float _friction;
        private float _currentForce;
        private float _floatTimer;

        public Motion(Rigidbody2D rigidbody, SinMovementStruct movement, SinMovementStruct floating, float friction)
        {
            _rigidbody = rigidbody;

            _movement = movement;
            _floating = floating;
            _friction = friction;

            _currentForce = _movement.Force;
            _floatTimer = 0;
        }

        public void HandleMovement(Vector3 rightDirection, Vector3 upDirection, float lifeTime, float currentTime)
        {
            //Calculate sine wave oscillation along the "up" direction
            var wave = Mathf.Sin(currentTime * _movement.Frequency) * _rigidbody.linearVelocity.magnitude * _movement.Intensity;
            _rigidbody.linearVelocity = (Vector2)upDirection * wave;

            _currentForce = Mathf.Lerp(0, _movement.Force, currentTime / lifeTime);
            _rigidbody.AddForce(rightDirection * _currentForce, ForceMode2D.Impulse);

            if (_rigidbody.linearVelocity.magnitude < 0.05f)
                _rigidbody.linearVelocity = Vector2.zero;
        }

        public void HandleFloat(float fixedDeltaTime)
        {
            if (_floatTimer > 0)
            {
                var verticalVelocity = Mathf.Sin((_floatTimer)* _floating.Frequency) * _floating.Intensity;
                _rigidbody.linearVelocity = new Vector2(0, verticalVelocity);
                _floatTimer += fixedDeltaTime;
            }
            else
            {
                if (_rigidbody.linearVelocity.magnitude > 0.5f)
                    _rigidbody.AddForce(-_rigidbody.linearVelocity * _friction, ForceMode2D.Force);
                else
                {
                    _rigidbody.linearVelocity = Vector2.zero;
                    _floatTimer = 1;
                }
            }
        }

        public void Motion_ObjectCaptured() => _rigidbody.linearVelocity = new Vector2(0, _floating.Force);
    }
}