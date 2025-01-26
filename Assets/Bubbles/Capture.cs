namespace Bubbles
{
    using UnityEngine;
    using System;

    public struct CaptureStruct
    {
        public BaseEnemy Enemy;
        public Transform Transform;
        public Collider2D Collider;
        public Rigidbody2D Rigidbody;
        public SpriteRenderer Renderer;
    }

    public class Capture
    {
        public Transform CaptureTarget => _struct.Transform;

        private readonly float _alpha;

        private CaptureStruct _struct;

        public event Action Captured;

        public Capture(float alpha)
        {
            _alpha = alpha;
        }

        public void Cleanup()
        {
            Captured = null;
        }

        public void Follow()
        {
            _struct.Transform.localPosition = Vector2.zero;
        }

        public void OnCaptured(Transform thisTransform, Transform capturedTransform)
        {
            _struct = new()
            {
                Enemy = capturedTransform.GetComponent<BaseEnemy>(),
                Transform = capturedTransform,
                Collider = capturedTransform.GetComponent<Collider2D>(),
                Rigidbody = capturedTransform.GetComponent<Rigidbody2D>(),
                Renderer = capturedTransform.GetComponentInChildren<SpriteRenderer>(),
            };

            ChangeCapturedObjectAlpha(_alpha);

            _struct.Transform.SetParent(thisTransform);
            _struct.Transform.localPosition = Vector2.zero;
            _struct.Collider.enabled = false;
            _struct.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;

            if (_struct.Enemy)
                _struct.Enemy.Bubble();

            Captured?.Invoke();
        }

        public void OnReleased()
        { 
            ChangeCapturedObjectAlpha(1);

            var isEnemy = _struct.Enemy;
            if (isEnemy)
                _struct.Enemy.Stun();

            _struct.Transform.SetParent(null);
            _struct.Collider.enabled = true;
            _struct.Transform = null;

            var constraints = isEnemy
                ? RigidbodyConstraints2D.FreezeRotation
                : RigidbodyConstraints2D.None;
            _struct.Rigidbody.constraints = constraints;
        }

        private void ChangeCapturedObjectAlpha(float alpha)
        {
            var colour = _struct.Renderer.color;
            colour.a = alpha;
            _struct.Renderer.color = colour;
        }
    }
}