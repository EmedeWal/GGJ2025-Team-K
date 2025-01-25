using System;
using UnityEngine;

public class RedKoopa : BaseEnemy
{
    /// <summary>
    /// Enemy that walks left and right, but will not voluntarily drop off the platform it is on
    /// Could also be made so it can ignore ledges and just walk left and right but don't know if we'll use that
    /// 
    /// Needs to have an enemy spawner set in editor if hasSpawner is true
    /// </summary>

    //void Start()
    //{
    //    Rigidbody = GetComponent<Rigidbody2D>();
    //    staysOnPlatform = true;
    //}

    //protected override void FixedUpdate()
    //{
    //    base.FixedUpdate();
    //}

    // It can do a lil death animation maybe, like falling down off-the-screen while spinning to keep it simple. In any case, it dies
    // public void Death()
    // {

    //     /*
    //     //Was trying to do a death animation, wasn't working so shelving this for now
    //     status = "STUNNED";
    //     GetComponent<Collider2D>().enabled = false;
    //     Rigidbody.linearVelocity = new Vector2(0, Rigidbody.linearVelocity.y);
    //     Rigidbody.AddForce(new Vector2(0, 50f));
    //     */
    // }

}
