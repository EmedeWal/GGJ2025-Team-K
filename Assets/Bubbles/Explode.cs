namespace Bubbles
{
    using UnityEngine;

    public class Explode
    {
        private readonly float _force;
        private readonly float _radius;

        public Explode(float force, float radius)
        {
            _force = force; 
            _radius = radius;
        }

        public void CastExplosion(Rigidbody2D thisRigidbody, Vector2 positon)
        {
            var colliders = Physics2D.OverlapCircleAll(positon, _radius);
            foreach (var coll in colliders)
                if (coll.TryGetComponent(out Rigidbody2D rigidbody) && rigidbody != thisRigidbody)
                    AddExplosionForce(rigidbody, _force, positon, _radius);
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
    }
}