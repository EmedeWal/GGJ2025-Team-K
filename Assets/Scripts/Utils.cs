using UnityEngine;

public static class Utils
{
    public static void SetRigidbody(Rigidbody2D rigidbody)
    {
        rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }
}