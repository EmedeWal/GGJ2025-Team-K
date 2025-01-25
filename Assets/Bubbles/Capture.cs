namespace Bubble
{
    using UnityEngine;

    public struct CaptureStruct
    {
        public Transform Transform;
        public Collider2D Collider;
        public Rigidbody2D Rigidbody;
        public SpriteRenderer Renderer;
    }

    public class Capture
    {
        public bool HasCapture => _struct.Transform;

        private readonly float _alpha;

        private CaptureStruct _struct;

        public Capture(float alpha)
        {
            _alpha = alpha;
        }

        public void Follow()
        {
            _struct.Transform.localPosition = Vector2.zero;
        }

        public void CaptureObject(Transform thisTransform, Transform capturedTransform)
        {
            _struct = new()
            {
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
        }

        public void ReleaseObject()
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
    }
}