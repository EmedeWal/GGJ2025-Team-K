using UnityEngine;

public class RedKoopa : MonoBehaviour, IKillable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // It can do a lil death animation maybe, like falling down off-the-screen while spinning to keep it simple. In any case, it dies
    public void die()
    {

    }

    void IKillable.onSpikeHit()
    {
        die();
    }
}
